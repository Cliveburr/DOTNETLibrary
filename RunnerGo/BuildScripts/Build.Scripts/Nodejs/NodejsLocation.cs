
namespace Build.Scripts.Nodejs
{
    public class NodejsLocation
    {
        private static string[] _possibleLocations = [
            @"C:\Program Files\nodejs"
        ];

        private static string FindNodejs()
        {
            return _possibleLocations
                .FirstOrDefault(p => Directory.Exists(p)) ?? _possibleLocations[0];
        }

        public static string GetNodePath()
        {
            return Path.Combine(FindNodejs(), "node.exe");
        }

        public static string GetNpmPath()
        {
            return Path.Combine(FindNodejs(), "npm.cmd");
        }
    }
}
