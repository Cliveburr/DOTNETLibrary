using Runner.Agent.Interface.Model;

namespace Runner.Agent.Version.Vers
{
    public static class VersionManager
    {
        public static string VersionName(UpdateVersionRequest request)
        {
            return $"v{request.Version}";
        }

        public static string VersionDirectory(UpdateVersionRequest request)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "vers", VersionName(request));
        }
    }
}
