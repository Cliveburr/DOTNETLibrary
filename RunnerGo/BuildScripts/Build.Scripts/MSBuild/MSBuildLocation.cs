using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Build.Scripts.MSBuild
{
    public static class MSBuildLocation
    {
        private static string MSBuildPath = @"MSBuild\Current\Bin\amd64\msbuild.exe";

        private static IEnumerable<string> FindAll()
        {
            var programFiles = Environment.ExpandEnvironmentVariables("%ProgramW6432%");
            foreach (var inst in FindAllVersion(programFiles))
            {
                yield return inst;
            }

            string programFilesX86 = Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%");
            foreach (var inst in FindAllVersion(programFilesX86))
            {
                yield return inst;
            }
        }

        private static IEnumerable<string> FindAllVersion(string path)
        {
            var visualStudioPath = Path.Combine(path, "Microsoft Visual Studio");
            var versions = Directory.GetDirectories(visualStudioPath);
            foreach (var version in versions)
            {
                var types = Directory.GetDirectories(version);

                foreach (var type in types)
                {
                    var msbuildPath = Path.Combine(type, MSBuildPath);

                    if (File.Exists(msbuildPath))
                    {
                        yield return msbuildPath;
                    }
                }
            }
        }

        public static string GetMSBuildPath()
        {
            var msbuildPath = FindAll()
                .FirstOrDefault();
            if (string.IsNullOrEmpty(msbuildPath))
            {
                throw new FileNotFoundException("MSBuild install not found!");
            }
            return msbuildPath;
        }
    }
}
