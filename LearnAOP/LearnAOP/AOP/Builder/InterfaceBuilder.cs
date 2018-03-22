using LearnAOP.AOP.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LearnAOP.AOP.Builder
{
    public class InterfaceBuilder : IBuilder
    {
        private Object _lockGet;
        private IDictionary<string, IInterfacePreBuild> _preBuild;

        public InterfaceBuilder()
        {
            _lockGet = new Object();
            _preBuild = new Dictionary<string, IInterfacePreBuild>();
        }

        public object Generate(ContainerType containerType)
        {
            var type = containerType.ResolvedType.ClassType;
            var typeName = type.FullName;

            lock (_lockGet)
            {
                if (!_preBuild.ContainsKey(typeName))
                {
                    _preBuild.Add(typeName, GeneratePreBuild(containerType));
                }
            }

            var preBuild = _preBuild[typeName];

            return preBuild.Generate();
        }

        private IInterfacePreBuild GeneratePreBuild(ContainerType containerType)
        {
            foreach (var method in containerType.ResolvedType.InterfaceType.GetMethods())
            {
                var attribs = AttributeHelper.GetAttributes<InterceptionAttribute>(method);

                if (attribs.Any())
                    return new VirtualPreBuild(containerType);

                foreach (var query in containerType.Container.Interception)
                {
                    if (query.IsApply(method))
                        return new VirtualPreBuild(containerType);
                }
            }

            return new DirectPreBuild(containerType);
        }
    }

    public interface IInterfacePreBuild
    {
        object Generate();
    }

    public class DirectPreBuild : IInterfacePreBuild
    {
        private ConstructorInfo _constructor;
        private Container _container;
        private Type _type;
        private ParameterInfo[] _params;

        public DirectPreBuild(ContainerType containerType)
        {
            _container = containerType.Container;
            _type = containerType.ResolvedType.ClassType;
            _constructor = _type.GetConstructors()[0];
            _params = _constructor.GetParameters();
        }

        public object Generate()
        {
            var paramsObjs = new List<object>();
            foreach (var pam in _params)
            {
                var obj = _container.Resolve(pam.ParameterType);
                paramsObjs.Add(obj);
            }
            return Activator.CreateInstance(_type, paramsObjs.ToArray());
        }
    }

    public class VirtualPreBuild : IInterfacePreBuild
    {
        private ConstructorInfo _constructor;
        private Container _container;
        private Type _type;
        private ParameterInfo[] _params;
        private InterfaceBuilderVirtual _virtual;

        public VirtualPreBuild(ContainerType containerType)
        {
            _container = containerType.Container;
            _type = containerType.ResolvedType.ClassType;
            _constructor = _type.GetConstructors()[0];
            _params = _constructor.GetParameters();
            _virtual = new InterfaceBuilderVirtual(containerType);
        }

        public object Generate()
        {
            var paramsObjsProxy = new List<object>();
            foreach (var pam in _params)
            {
                var obj = _container.Resolve(pam.ParameterType);
                paramsObjsProxy.Add(obj);
            }
            var proxy = Activator.CreateInstance(_type, paramsObjsProxy.ToArray());

            var paramsObjs = new List<object>
            {
                this,
                proxy
            };
            return Activator.CreateInstance(_virtual.GenerateType, paramsObjs.ToArray());
        }

        public void RunPreExecute(InterceptionRunContext context, int[] preExecuteList)
        {
            foreach (var index in preExecuteList)
            {
                _virtual.Interceptions[index].PreExecute(context);
            }
        }

        public void RunErrorExecute(InterceptionRunContext context, int[] errorExecuteList)
        {
            foreach (var index in errorExecuteList)
            {
                _virtual.Interceptions[index].ErrorExecute(context);
            }
        }

        public void RunPosExecute(InterceptionRunContext context, int[] posExecuteList)
        {
            foreach (var index in posExecuteList)
            {
                _virtual.Interceptions[index].PosExecute(context);
            }
        }
    }
}