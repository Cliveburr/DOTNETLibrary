﻿using Runner.Business.Model.Nodes.Types;
using Runner.Script.Interface.Model.Data;
using Runner.Script.Interface.Scripts;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Runner.Agent.Version.Scripts
{
    public class ScriptIsolation
    {
        private string _rootPath;
        private List<string> _assemblyPaths;
        private List<ScriptSet> _result;

        public ScriptIsolation(string rootPath)
        {
            _rootPath = rootPath;
            _assemblyPaths = Directory.GetFiles(rootPath, "*.dll", SearchOption.AllDirectories)
                .Where(s => !s.EndsWith("Runner.Script.Interface.dll"))
                .ToList();
            _result = new List<ScriptSet>();
        }

        public List<ScriptSet> Execute(StringBuilder warnings)
        {
            WeakReference contextRef;
            ExecuteAndUnload(out contextRef, warnings);

            for (int i = 0; contextRef.IsAlive && (i < 10); i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            var timeout = DateTime.Now.AddMilliseconds(60000);
            while (contextRef.IsAlive && DateTime.Now < timeout)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();

                Task.Delay(100).Wait();
            }

            if (contextRef.IsAlive)
            {
                warnings.AppendLine("Internal - Context still alive!");
            }

            return _result;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ExecuteAndUnload(out WeakReference contextRef, StringBuilder warnings)
        {
            var context = new ScriptAssemblyLoadContext();
            contextRef = new WeakReference(context, true);

            var iscript = typeof(IScript);
            var rootPathLen = _rootPath.Length;

            try
            {
                foreach (var assemblyPath in _assemblyPaths)
                {
                    var assembly = context.LoadFromAssemblyPath(assemblyPath);
                    if (assembly is null)
                    {
                        warnings.AppendLine("Internal - Assembly not found! " + assemblyPath);
                        continue;
                    }

                    var scripts = assembly.GetTypes()
                        .Where(p => iscript.IsAssignableFrom(p));

                    foreach (var script in scripts)
                    {
                        var fullTypeName = script.Namespace is null ?
                            script.Name :
                            $"{script.Namespace}.{script.Name}";

                        var scriptAttr = script.GetCustomAttribute<ScriptAttribute>();
                        if (scriptAttr is not null)
                        {
                            _result.Add(new ScriptSet
                            {
                                Name = scriptAttr.Name,
                                Version = scriptAttr.Version,
                                Assembly = assemblyPath.Substring(rootPathLen),
                                FullTypeName = fullTypeName,
                                InputTypes = scriptAttr.InputTypes?
                                    .Select(i => new Business.Datas.Model.DataTypeProperty
                                    {
                                        Name = i.Name,
                                        Type = (Business.Datas.Model.DataTypeEnum)i.Type,
                                        Default = i.Default,
                                        IsRequired = i.IsRequired
                                    }).ToList() ?? new List<Business.Datas.Model.DataTypeProperty>(),
                                OutputTypes = scriptAttr.OutputTypes?
                                    .Select(i => new Business.Datas.Model.DataTypeProperty
                                    {
                                        Name = i.Name,
                                        Type = (Business.Datas.Model.DataTypeEnum)i.Type,
                                        Default = i.Default,
                                        IsRequired = i.IsRequired
                                    }).ToList() ?? new List<Business.Datas.Model.DataTypeProperty>()
                            });
                        }
                        else
                        {
                            warnings.AppendLine("Script missing ScriptAttribute: " + fullTypeName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                warnings.AppendLine("Error: " + ex.Message);
            }
            finally
            {
                context.Unload();
            }
        }
    }
}