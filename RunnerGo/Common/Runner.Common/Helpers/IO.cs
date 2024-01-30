
namespace Runner.Common.Helpers
{
    public static class IO
    {
        public static void ClearDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                return;
            }

            var directoryInfo = new DirectoryInfo(path);

            foreach (var file in directoryInfo.GetFiles())
            {
                try
                {
                    file.Delete();
                }
                catch
                { 
                    Thread.Sleep(1);
                    file.Delete();
                }
            }
            foreach (var dir in directoryInfo.GetDirectories())
            {
                try
                {
                    dir.Delete(true);
                }
                catch
                {
                    Thread.Sleep(1);
                    dir.Delete(true);
                }
            }
        }

        public static void DeleteDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }

            var directoryInfo = new DirectoryInfo(path);

            foreach (var file in directoryInfo.GetFiles())
            {
                try
                {
                    file.Delete();
                }
                catch
                {
                    Thread.Sleep(1);
                    file.Delete();
                }
            }
            foreach (var dir in directoryInfo.GetDirectories())
            {
                try
                {
                    dir.Delete(true);
                }
                catch
                {
                    Thread.Sleep(1);
                    dir.Delete(true);
                }
            }

            try
            {
                directoryInfo.Delete(true);
            }
            catch
            {
                Thread.Sleep(1);
                directoryInfo.Delete(true);
            }
        }
    }
}
