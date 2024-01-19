using Runner.Agent.Interface.Model;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Agent.Scripts
{
    public static class ScriptsManager
    {
        public static string ScriptDirectory(RunScriptRequest request)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src", request.Id, $"v{request.Version}");
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

            using (var memoryStream = new MemoryStream(getScriptResponse.ZipContent))
            using (var zip = new ZipArchive(memoryStream))
            {
                foreach (var file in zip.Entries)
                {
                    var fullFileName = Path.Combine(scriptPath, file.FullName);
                    file.ExtractToFile(fullFileName);
                }
            }
        }
    }
}
