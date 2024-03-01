
namespace Runner.Script.Interface.Workspaces
{
    public class WorkspaceFolder
    {
        public string Path { get; init; }

        public WorkspaceFolder(string path)
        {
            Path = path;
        }

        public void ClearThisPath()
        {
            ClearDirectory(Path);
        }

        public void ClearSubPath(string subPath)
        {
            ClearDirectory(System.IO.Path.Combine(Path, subPath));
        }

        public void DeleteThisPath()
        {
            DeleteDirectory(Path);
        }

        public void DeleteSubPath(string subPath)
        {
            DeleteDirectory(System.IO.Path.Combine(Path, subPath));
        }

        public void ClearDirectory(string path)
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

        public void DeleteDirectory(string path)
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
