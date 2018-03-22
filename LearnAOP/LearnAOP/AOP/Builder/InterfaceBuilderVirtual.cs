using LearnAOP.AOP.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LearnAOP.AOP.Builder
{
    class InterfaceBuilderVirtual
    {
        public Type GenerateType { get; private set; }
        public IList<IInterception> Interceptions { get; private set; }

        public InterfaceBuilderVirtual(ContainerType containerType)
        {
            Interceptions = new List<IInterception>();
            var interfaceType = containerType.ResolvedType.InterfaceType;
            var interfaceName = interfaceType.Name;

            var code = GenerateCode(containerType);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);

            //var references = new MetadataReference[]
            //{
            //    //MetadataReference.CreateFromFile(typeof(System.Runtime.GCSettings).Assembly.Location),
            //    MetadataReference.CreateFromFile(typeof(Object).Assembly.Location),
            //    MetadataReference.CreateFromFile(typeof(Container).Assembly.Location)
            //};
            var references = AppDomain.CurrentDomain.GetAssemblies()
                .Select(a => MetadataReference.CreateFromFile(a.Location))
                .ToArray();

            var compilation = CSharpCompilation.Create(
                $"AssemblyProxy{interfaceName}",
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            Assembly assembly = null;

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);

                if (!result.Success)
                {
                    var failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    throw new Exception("Error generating the virtual assembly!");
                }
                else
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    assembly = Assembly.Load(ms.ToArray());
                }
            }

            GenerateType = assembly.GetType("AOP.Virtual." + interfaceName);
        }

        private string GenerateCode(ContainerType containerType)
        {
            var interfaceType = containerType.ResolvedType.InterfaceType;

            return string.Format(@"
using System;
using System.Collections.Generic;
//using System.IO;
//using System.Runtime.Serialization.Formatters;
//using System.Runtime.Serialization.Formatters.Binary;
//using LearnAOP.AOP;
using LearnAOP.AOP.Builder;

namespace AOP.Virtual
{{
    public class {0} : {1}
    {{
        private VirtualPreBuild _virtual;
        private {1} _proxy;

        public {0}(VirtualPreBuild virt, {1} proxy)
        {{
            _virtual = virt;
            _proxy = proxy;
        }}
        {2}
    }}
}}
", interfaceType.Name, interfaceType.FullName, GenerateMethods(containerType));
        }

        private string GenerateMethods(ContainerType containerType)
        {
            var interfaceType = containerType.ResolvedType.InterfaceType;
            var container = containerType.Container;

            var ret = new StringBuilder();
            foreach (var method in interfaceType.GetMethods())
            {
                var interceptions = AttributeHelper.GetAttributes<InterceptionAttribute>(method)
                    .Cast<IInterception>()
                    .Concat(container.Interception.Where(i => i.IsApply(method))
                        .Cast<IInterception>());

                var pre = new List<int>();
                var error = new List<int>();
                var pos = new List<int>();

                foreach(var interception in interceptions)
                {
                    var index = Interceptions.IndexOf(interception);
                    if (index < 0)
                    {
                        Interceptions.Add(interception);
                        index = Interceptions.Count - 1;
                    }

                    if (interception.HasPreExecute)
                        pre.Add(index);

                    if (interception.HasErrorExecute)
                        error.Add(index);

                    if (interception.HasPosExecute)
                        pos.Add(index);
                }

                var helper = new MethodHelper(interfaceType, method, pre.ToArray(), error.ToArray(), pos.ToArray());

                ret.Append(GenerateMethod(helper));
            }
            return ret.ToString();
        }

        private string GenerateMethod(MethodHelper method)
        {
            return string.Format(@"
        public {0} {1}({2})
        {{
            {3}
            {4}
            {5}
        }}
", method.ReturnTypeString, method.Name, method.ParamsFull, GeneratePreExecution(method), GenerateExecution(method), GeneratePosExecution(method));
        }

        private string GeneratePreExecution(MethodHelper method)
        {
            var ret = new StringBuilder();
            if (method.HasReturn)
            {
                ret.AppendLine($"{method.ReturnTypeString} ret = null;");
            }

            if (method.HasPreExecution)
            {
                ret.AppendLine(string.Format(@"
            var context = new InterceptionRunContext
            {{
                Object = this,
                MethodName = ""{0}"",
                Parameters = new object[] {{ {1} }}
            }};
            _virtual.RunPreExecute(context, new int[] {{ {2} }});",
                method.Name, method.Params, string.Join(", ", method.PreExecution)));
            }
            return ret.ToString();
        }

        private string GenerateExecution(MethodHelper method)
        {
            var execut = method.HasReturn ?
                string.Format(@"ret = _proxy.{0}({1});", method.Name, method.Params) :
                string.Format(@"_proxy.{0}({1});", method.Name, method.Params);

            if (method.HasErrorExecution)
            {
                return string.Format(@"
            try
            {{
                {0}
            }}
            catch (Exception err)
            {{
                context.Exception = err;
                _virtual.RunErrorExecute(context, new int[] {{ {1} }});
                if (context.RaiseException)
                    throw err;
            }}
", execut, string.Join(", ", method.ErrorExecution));
            }
            else
            {
                return execut;
            }
        }

        private string GeneratePosExecution(MethodHelper method)
        {
            var ret = new StringBuilder();
            if (method.HasPosExecution)
            {
                ret.AppendLine(string.Format(@"
                _virtual.RunPosExecute(context, new int[] {{ {0} }});",
                string.Join(", ", method.PosExecution)));
            }

            if (method.HasReturn)
            {
                return string.Format(@"
                return ret;");
            }
            return ret.ToString();
        }
    }

    class MethodHelper
    {
        public string Name { get; set; }
        public bool HasReturn { get; set; }
        public string ReturnTypeString { get; set; }
        public string ParamsFull { get; set; }
        public string Params { get; set; }
        public bool HasPreExecution { get; set; }
        public int[] PreExecution { get; set; }
        public bool HasErrorExecution { get; set; }
        public int[] ErrorExecution { get; set; }
        public bool HasPosExecution { get; set; }
        public int[] PosExecution { get; set; }

        public MethodHelper(Type type, MethodInfo method, int[] preExecution, int[] errorExecution, int[] posExecution)
        {
            Name = method.Name;
            HasReturn = !method.ReturnType.Equals(typeof(void));
            ReturnTypeString = HasReturn ? method.ReturnType.FullName : "void";
            ParamsFull = string.Join(", ", method.GetParameters().Select(p => GetParameterType(p) + " " + p.Name).ToArray());
            Params = string.Join(", ", method.GetParameters().Select(p => p.Name).ToArray());
            HasPreExecution = preExecution.Any();
            PreExecution = preExecution;
            HasErrorExecution = errorExecution.Any();
            ErrorExecution = errorExecution;
            HasPosExecution = posExecution.Any();
            PosExecution = posExecution;
        }

        private string GetSafeType(Type type)
        {
            if (type.Equals(typeof(void)))
                return "void";
            else
                return type.FullName;
        }

        private string GetParameterType(ParameterInfo parameter)
        {
            var t = parameter.ParameterType;

            if (t.AssemblyQualifiedName.StartsWith("System.Collections.Generic.List") && t.IsGenericType)
            {
                return string.Format("List<{0}>", string.Join(", ", t.GetGenericArguments().Select(g => GetSafeType(g))));
            }
            else
            {
                return t.FullName;
            }
        }
    }
}