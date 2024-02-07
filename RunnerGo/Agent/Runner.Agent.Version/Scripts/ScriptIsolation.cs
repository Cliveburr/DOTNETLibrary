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
        private readonly RunScriptRequest _request;
        private readonly Func<string, Task> _log;
        private RunScriptResponse? _result;

        public ScriptIsolation(RunScriptRequest request, Func<string, Task> log)
        {
            _request = request;
            _log = log;
        }

        public async Task<RunScriptResponse> Execute(CancellationToken cancellationToken)
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
                await _log("AssemblyLoadContext not unloaded!");
            }

            return _result ?? new RunScriptResponse
            {
                IsSuccess = false,
                ErrorMessage = "ExecuteAndUnload dont return result!"
            };
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
                _result = new RunScriptResponse
                {
                    IsSuccess = false,
                    ErrorMessage = $"File not found: \"{_request.Assembly}\"!"
                };
                return;
            }

            var assembly = context.LoadFromAssemblyPath(assemblyPath);
            if (assembly is null)
            {
                _result = new RunScriptResponse
                {
                    IsSuccess = false,
                    ErrorMessage = $"Invalid assemblyPath: \"{assemblyPath}\"!"
                };
                return;
            }

            var type = assembly.GetType(_request.FullTypeName, false, true);
            
            if (type is null)
            {
                _result = new RunScriptResponse
                {
                    IsSuccess = false,
                    ErrorMessage = $"Cannot find: \"{_request.FullTypeName}\" class!"
                };
                return;
            }

            //var runMethod = type.GetMethod("Run");
            //if (runMethod is null)
            //{
            //    _result = new ExecuteResult
            //    {
            //        IsSuccess = false,
            //        ErrorMessage = $"Class: \"{_request.FullTypeName}\" dont have Run method!"
            //    };
            //    return;
            //}

            var instance = Activator.CreateInstance(type) as IScript;
            if (instance is null)
            {
                _result = new RunScriptResponse
                {
                    IsSuccess = false,
                    ErrorMessage = $"Instance fail: \"{_request.FullTypeName}\"!"
                };
                return;
            }

            try
            {
                //var dataMap = _request.InputData?
                //    .Select(p => new ScriptDataProperty
                //    {
                //        Name = p.Name,
                //        Type = (ScriptDataTypeEnum)p.Type,
                //        Value = p.Value
                //    })
                //    .ToList();

                var scriptRunContext = new ScriptRunContext
                {
                    IsSuccess = true,
                    Data = MapDataInput(_request.InputData),
                    Log = _log,
                    CancellationToken = cancellationToken
                };

                //var taskAwait = runMethod.Invoke(instance, [scriptRunContext]) as Task;
                //if (taskAwait is not null)
                //{
                //    taskAwait.Wait();
                //}
                instance.Run(scriptRunContext).Wait();

                _result = new RunScriptResponse
                {
                    IsSuccess = scriptRunContext.IsSuccess,
                    ErrorMessage = scriptRunContext.ErrorMessage,
                    OutputData = MapDataOutput(scriptRunContext.Data)
                    
                };
            }
            catch (Exception ex)
            {
                _result = new RunScriptResponse
                {
                    IsSuccess = false,
                    ErrorMessage = $"Run Error: {ex}"
                };
            }
            finally
            {
                context.Unload();
            }
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
            var properties = data.MapTo(s => new AgentDataPropertyString
                {
                    Name = s.Property.Name,
                    Type = (Interface.Model.Data.AgentDataTypeEnum)s.Property.Type,
                    Value = s.Property.Value
                });

            var transfer = new AgentDataTransfer();

            var strings = properties
                .Where(p => p.Type == DataTypeEnum.String)
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
                .Where(p => p.Type == DataTypeEnum.StringList)
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
                .Where(p => p.Type == DataTypeEnum.NodePath)
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
