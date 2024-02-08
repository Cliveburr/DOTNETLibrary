using Runner.Agent.Interface.Model;
using Runner.Agent.Interface.Model.Data;
using Runner.Script.Interface.Model.Data;
using Runner.Script.Interface.Scripts;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;

namespace Runner.Agent.Version.Scripts
{
    public class ScriptIsolation
    {
        public RunScriptResponse? Result { get; set; }
        public Exception? Exception { get; set; }

        private readonly RunScriptRequest _request;
        private readonly Func<string, Task> _log;

        public ScriptIsolation(RunScriptRequest request, Func<string, Task> log)
        {
            _request = request;
            _log = log;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Execute(CancellationToken cancellationToken)
        {
            WeakReference contextRef;
            ExecuteAndUnload(out contextRef, cancellationToken);

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
                //throw new Exception("AssemblyLoadContext not unloaded!");
                _log("AssemblyLoadContext not unloaded!").Wait();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ExecuteAndUnload(out WeakReference contextRef, CancellationToken cancellationToken)
        {
            var scriptRoot = ScriptsManager.ScriptDirectory(_request);
            var assemblyPath = Path.Combine(scriptRoot, _request.Assembly.Trim('\\'));

            var myAssembly = typeof(ScriptIsolation).Assembly;
            var agentVersionContext = AssemblyLoadContext.GetLoadContext(myAssembly)!;
            var context = new ScriptAssemblyLoadContext(scriptRoot, agentVersionContext);
            contextRef = new WeakReference(context, true);

            if (!File.Exists(assemblyPath))
            {
                Exception = new FileNotFoundException($"File not found: \"{_request.Assembly}\"!");
                context.Unload();
                return;
            }

            var assembly = context.LoadFromAssemblyPath(assemblyPath);
            if (assembly is null)
            {
                Exception = new TypeLoadException($"Invalid assemblyPath: \"{assemblyPath}\"!");
                context.Unload();
                return;
            }

            var type = assembly.GetType(_request.FullTypeName, false, true);
            
            if (type is null)
            {
                Exception = new ArgumentException($"Cannot find: \"{_request.FullTypeName}\" class!");
                context.Unload();
                return;
            }

            var instance = Activator.CreateInstance(type) as IScript;
            if (instance is null)
            {
                Exception = new Exception($"Instance fail: \"{_request.FullTypeName}\"!");
                context.Unload();
                return;
            }

            //try
            //{
                var scriptRunContext = new ScriptRunContext
                {
                    Data = MapDataInput(_request.InputData),
                    Log = _log,
                    CancellationToken = cancellationToken
                };

                //instance.Run(scriptRunContext).Wait();
                instance.Run(scriptRunContext)
                    .ContinueWith(t =>
                    {
                        //context.Unload();
                        if (t.IsFaulted)
                        {
                            Exception = t.Exception;
                        }
                        else
                        {
                            Result = new RunScriptResponse
                            {
                                OutputData = MapDataOutput(scriptRunContext.Data)
                            };
                        }
                    })
                    .Wait();
                    //.ConfigureAwait(true)
                    //.GetAwaiter()
                    //.GetResult();
            //}
            //catch (Exception ex)
            //{
            //    Exception = ex.InnerException ?? ex;
            //}
            //finally
            //{
                context.Unload();
            //}
        }

        private ScriptData MapDataInput(AgentDataTransfer? transfer)
        {
            var properties = new List<ScriptDataProperty>();

            if (transfer is not null)
            {
                if (transfer.Strings is not null && transfer.Strings.Length > 0)
                {
                    properties.AddRange(transfer.Strings
                        .Select(s => new ScriptDataProperty
                        {
                            Name = s.Name,
                            Type = ScriptDataTypeEnum.String,
                            Value = s.Value,
                        }));
                }

                if (transfer.StringLists is not null && transfer.StringLists.Length > 0)
                {
                    properties.AddRange(transfer.StringLists
                        .Select(s => new ScriptDataProperty
                        {
                            Name = s.Name,
                            Type = ScriptDataTypeEnum.StringList,
                            Value = s.Value,
                        }));
                }

                if (transfer.NodePaths is not null && transfer.NodePaths.Length > 0)
                {
                    properties.AddRange(transfer.NodePaths
                        .Select(s => new ScriptDataProperty
                        {
                            Name = s.Name,
                            Type = ScriptDataTypeEnum.NodePath,
                            Value = s.Value,
                        }));
                }
            }

            return new ScriptData(properties);
        }

        private AgentDataTransfer? MapDataOutput(ScriptData data)
        {
            var properties = data.GetStates()
                .Where(s => s.State != ScriptDataStateType.Pristine)
                .Select(s => s.Property)
                .ToList();

            var transfer = new AgentDataTransfer();

            var strings = properties
                .Where(p => p.Type == ScriptDataTypeEnum.String)
                .Select(p => new AgentDataPropertyString
                {
                    Name = p.Name,
                    Value = p.Value as string
                })
                .ToArray();
            if (strings.Any())
            {
                transfer.Strings = strings;
            }

            var stringLists = properties
                .Where(p => p.Type == ScriptDataTypeEnum.StringList)
                .Select(p => new AgentDataPropertyStringList
                {
                    Name = p.Name,
                    Value = p.Value as string[]
                })
                .ToArray();
            if (stringLists.Any())
            {
                transfer.StringLists = stringLists;
            }

            var nodePaths = properties
                .Where(p => p.Type == ScriptDataTypeEnum.NodePath)
                .Select(p => new AgentDataPropertyNodePath
                {
                    Name = p.Name,
                    Value = p.Value as string
                })
                .ToArray();
            if (nodePaths.Any())
            {
                transfer.NodePaths = nodePaths;
            }

            return transfer;
        }
    }
}
