
namespace Build.Scripts.Nuget
{
    public static class NugetLocation
    {
        public static string GetNugetPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Nuget", "exe", "nuget.exe");
        }
    }
}
