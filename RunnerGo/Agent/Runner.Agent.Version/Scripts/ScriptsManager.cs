using Runner.Agent.Interface.Model;
using Runner.Common.Helpers;

namespace Runner.Agent.Version.Scripts
{
    public static class ScriptsManager
    {
        public static string ScriptDirectory(RunScriptRequest request)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src", request.ScriptContentId);
        }

        public static bool CheckIfExist(RunScriptRequest request)
        {
            var scriptPath = ScriptDirectory(request);
            return Directory.Exists(scriptPath);
        }

        public static void Create(RunScriptRequest request, GetScriptResponse getScriptResponse)
        {
            var scriptPath = ScriptDirectory(request);

            if (!Directory.Exists(scriptPath))
            {
                Directory.CreateDirectory(scriptPath);
            }
            else
            {
                IO.ClearDirectory(scriptPath);
            }

            Zip.Descompat(getScriptResponse.Content, scriptPath);
        }
    }
}
