using System.Text.Json;

namespace Runner.Agent.vers
{
    public static class VersionInfo
    {
        public static string ReadVersionActual()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "vers", "versionsinfo.json");
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }
            
            using (var streamReader = File.OpenRead(path))
            {
                var versionInfo = JsonSerializer.Deserialize<VersionInfoModel>(streamReader);
                if (versionInfo is null)
                {
                    throw new Exception("Invalid versionsinfo.json");
                }

                if (versionInfo.versionActual < versionInfo.versions.Length)
                {
                    return versionInfo.versions[versionInfo.versionActual];
                }
                else
                {
                    throw new Exception("Invalid versionActual");
                }
            }
        }
    }

    public class VersionInfoModel
    {
        public int versionActual { get; set; }
        public required string[] versions { get; set; }
    }
}
