
namespace Runner.Common.Helpers
{
    public static class TempDirectory
    {
        private static object _lock = new object();

        public static string BaseTempDirectory()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "_temp");
        }

        public static TempDirectoryItem Create(string alias)
        {
            var basePath = Path.Combine(BaseTempDirectory(), DateTime.UtcNow.ToString("yyyyMMdd_HHmmssf"));
            var iteration = 0;

            var path = "";
            lock (_lock)
            {
                do
                {
                    path = $"{basePath}_{iteration++}_{alias}";
                } while (Directory.Exists(path));
            }

            Directory.CreateDirectory(path);

            return new TempDirectoryItem(path);
        }
    }

    public class TempDirectoryItem : IDisposable
    {
        public string Path { get; init; }

        public TempDirectoryItem(string path)
        {
            Path = path;
        }

        public void Dispose()
        {
            IO.DeleteDirectory(Path);
        }
    }
}
