using Runner.Agent.Interface.Model;
using Runner.Agent.Version.Scripts;
using Runner.Script.Interface.Data;
using Runner.Script.Interface.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Agent.Version.Isolation
{
    public static class ScriptIsolation
    {
        public static async Task<ExecuteResult> Execute(RunScriptRequest request, Func<string, Task> log, CancellationToken cancellationToken)
        {
            var root = ScriptsManager.ScriptDirectory(request);
            var assemblyPath = Path.Combine(root, request.Assembly);

            if (!File.Exists(assemblyPath))
            {
                return new ExecuteResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"File not found: \"{assemblyPath}\"!"
                };
            }

            var context = new AssemblyLoadContext(null, true);
            var assembly = context.LoadFromAssemblyPath(assemblyPath);
            if (assembly is null)
            {
                return new ExecuteResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Assembly not load: \"{assemblyPath}\"!"
                };
            }

            var type = assembly.GetType(request.Type, false, true);
            if (type is null)
            {
                return new ExecuteResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Type not found: \"{request.Type}\"!"
                };
            }

            var instance = Activator.CreateInstance(type) as IScript;
            if (instance is null)
            {
                return new ExecuteResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Instance fail: \"{request.Type}\"!"
                };
            }

            var scriptRunContext = new ScriptRunContext
            {
                IsSuccess = true,
                ContinueOnError = false,
                Input = new DataReader(request.Input),
                Output = new DataWriter(),
                Log = log,
                CancellationToken = cancellationToken
            };

            try
            {
                await instance.Run(scriptRunContext);
            }
            catch (Exception ex)
            {
                return new ExecuteResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Run Error: {ex}"
                };
            }
            finally
            {
                context.Unload();
                context = null;
            }

            return new ExecuteResult
            {
                IsSuccess = scriptRunContext.IsSuccess,
                ContinueOnError = scriptRunContext.ContinueOnError,
                ErrorMessage = scriptRunContext.ErrorMessage,
                Output = scriptRunContext.Output
            };
        }
    }
}
