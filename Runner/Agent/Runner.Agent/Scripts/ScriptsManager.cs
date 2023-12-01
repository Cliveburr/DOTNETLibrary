using Runner.Agent.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Agent.Scripts
{
    public static class ScriptsManager
    {
        private static string ScriptDirectory(RunScriptRequest request)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts", request.Id, $"v{request.Version}");
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
                Helpers.IO.ClearDirectory(scriptPath);
            }

            foreach (var file in getScriptResponse.Files)
            {
                var fullFileName = Path.Combine(scriptPath, file.Name);
                File.WriteAllBytes(fullFileName, file.Content);

                var fileInfo = new FileInfo(fullFileName);
                fileInfo.CreationTime = file.CreateDateTime;
                fileInfo.LastWriteTime = file.LastUpdateDateTime;
            }
        }
    }
}
