using Runner.Agent.Interface.Model;
using Runner.Script.Interface.Model.Data;
using Runner.Script.Interface.Scripts;
using System.Runtime.CompilerServices;

namespace Runner.Agent.Version.Scripts
{
    public class ScriptIsolation
    {
        private readonly RunScriptRequest _request;
        private readonly Func<string, Task> _log;
        private ExecuteResult? _result;

        public ScriptIsolation(RunScriptRequest request, Func<string, Task> log)
        {
            _request = request;
            _log = log;
        }

        public async Task<ExecuteResult> Execute(CancellationToken cancellationToken)
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

            return _result ?? new ExecuteResult
            {
                IsSuccess = false,
                ErrorMessage = "ExecuteAndUnload dont return result!"
            };
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ExecuteAndUnload(out WeakReference contextRef, CancellationToken cancellationToken)
        {
            var context = new ScriptAssemblyLoadContext();
            contextRef = new WeakReference(context, true);

            var scriptRoot = ScriptsManager.ScriptDirectory(_request);
            var assemblyPath = Path.Combine(scriptRoot, _request.Assembly);

            if (!File.Exists(assemblyPath))
            {
                _result = new ExecuteResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"File not found: \"{assemblyPath}\"!"
                };
                return;
            }

            var assembly = context.LoadFromAssemblyPath(assemblyPath);
            if (assembly is null)
            {
                _result = new ExecuteResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Invalid assemblyPath: \"{assemblyPath}\"!"
                };
                return;
            }

            var type = assembly.GetType(_request.Type, false, true);
            if (type is null)
            {
                _result = new ExecuteResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Cannot find: \"{_request.Type}\" class!"
                };
                return;
            }

            var instance = Activator.CreateInstance(type) as IScript;
            if (instance is null)
            {
                _result = new ExecuteResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Instance fail: \"{_request.Type}\"!"
                };
                return;
            }

            try
            {
                var dataMap = _request.Data
                    .Select(p => new DataProperty
                    {
                        Name = p.Name,
                        Type = (DataTypeEnum)p.Type,
                        Value = p.Value
                    })
                    .ToList();

                var scriptRunContext = new ScriptRunContext
                {
                    IsSuccess = true,
                    ContinueOnError = false,
                    Data = new Data(dataMap),
                    Log = _log,
                    CancellationToken = cancellationToken
                };

                instance.Run(scriptRunContext).Wait();

                _result = new ExecuteResult
                {
                    IsSuccess = scriptRunContext.IsSuccess,
                    ContinueOnError = scriptRunContext.ContinueOnError,
                    ErrorMessage = scriptRunContext.ErrorMessage,
                    Data = scriptRunContext.Data.MapTo(s =>
                        new Interface.Model.Data.DataState
                        {
                            Property = new Interface.Model.Data.DataProperty { Name = s.Property.Name, Type = (Interface.Model.Data.DataTypeEnum)s.Property.Type, Value = s.Property.Value },
                            State = (Interface.Model.Data.DataStateType)s.State
                        })
                };
            }
            catch (Exception ex)
            {
                _result = new ExecuteResult
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
    }
}
