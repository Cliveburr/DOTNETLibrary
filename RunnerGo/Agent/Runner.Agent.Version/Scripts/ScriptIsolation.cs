using Microsoft.Extensions.DependencyInjection;
using Runner.Agent.Interface.Model;
using Runner.Script.Interface.Data;
using Runner.Script.Interface.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

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
                var scriptRunContext = new ScriptRunContext
                {
                    IsSuccess = true,
                    ContinueOnError = false,
                    Input = new DataReader(_request.Input),
                    Output = new DataWriter(),
                    Log = _log,
                    CancellationToken = cancellationToken
                };

                instance.Run(scriptRunContext).Wait();

                _result = new ExecuteResult
                {
                    IsSuccess = scriptRunContext.IsSuccess,
                    ContinueOnError = scriptRunContext.ContinueOnError,
                    ErrorMessage = scriptRunContext.ErrorMessage,
                    Output = scriptRunContext.Output
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
