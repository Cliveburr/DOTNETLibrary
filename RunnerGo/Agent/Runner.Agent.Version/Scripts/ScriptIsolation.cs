using Runner.Agent.Interface.Model;
using Runner.Agent.Interface.Model.Data;
using Runner.Agent.Version.Workspaces;
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

            // voltar depois dos testes
            //var timeout = DateTime.Now.AddMilliseconds(60000);
            //while (contextRef.IsAlive && DateTime.Now < timeout)
            //{
            //    GC.Collect();
            //    GC.WaitForPendingFinalizers();

            //    Task.Delay(100).Wait();
            //}

            //if (contextRef.IsAlive)
            //{
            //    //throw new Exception("AssemblyLoadContext not unloaded!");
            //    _log("AssemblyLoadContext not unloaded!").Wait();
            //}
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

            var scriptRunContext = new ScriptRunContext
            {
                Data = new ScriptData(MapDataInput(_request.InputData)),
                Log = _log,
                CancellationToken = cancellationToken,
                Workspace = new WorkspaceManager(_request.FlowId)
            };

            instance.Run(scriptRunContext)
                .ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        Exception = t.Exception;
                    }
                    else
                    {
                        Result = new RunScriptResponse
                        {
                            OutputData = MapDataOutput(scriptRunContext.Data.GetOutput().ToList())
                        };
                    }
                })
                .Wait();
                    
            context.Unload();
        }

        private List<ScriptDataProperty>? MapDataInput(List<AgentDataProperty>? properties)
        {
            if (properties is null)
            {
                return null;
            }

            return properties
                .Select(p => new ScriptDataProperty
                {
                    Name = p.Name,
                    Type = (ScriptDataTypeEnum)p.Type,
                    Value = p.Value is null ?
                        null :
                        new ScriptDataValue
                        {
                            StringValue = p.Value.StringValue,
                            IntValue = p.Value.IntValue,
                            StringListValue = p.Value.StringListValue,
                            NodePath = p.Value.NodePath,
                            DataExpand = MapDataInput(p.Value.DataExpand)
                        }
                })
                .ToList();
        }

        private List<AgentDataProperty>? MapDataOutput(List<ScriptDataProperty>? properties)
        {
            if (properties is null || !properties.Any())
            {
                return null;
            }

            return properties
                .Select(p => new AgentDataProperty
                {
                    Name = p.Name,
                    Type = (AgentDataTypeEnum)p.Type,
                    Value = p.Value is null ?
                        null :
                        new AgentDataValue
                        {
                            StringValue = p.Value.StringValue,
                            IntValue = p.Value.IntValue,
                            StringListValue = p.Value.StringListValue,
                            NodePath = p.Value.NodePath,
                            DataExpand = MapDataOutput(p.Value.DataExpand)
                        }
                })
                    .ToList();
        }
    }
}
