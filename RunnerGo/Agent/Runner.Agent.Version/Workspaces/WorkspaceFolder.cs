using Runner.Common.Helpers;
using Runner.Script.Interface.Scripts;

namespace Runner.Agent.Version.Workspaces
{
    public class WorkspaceFolder : IWorkspaceFolder
    {
        public string Path { get; init; }

        public WorkspaceFolder(string path)
        {
            Path = path;
        }

        public void ClearDirectory()
        {
            IO.ClearDirectory(Path);
        }

        public void ClearDirectory(string subPath)
        {
            IO.ClearDirectory(System.IO.Path.Combine(Path, subPath));
        }

        public void DeleteDirectory()
        {
            IO.DeleteDirectory(Path);
        }

        public void DeleteDirectory(string subPath)
        {
            IO.DeleteDirectory(System.IO.Path.Combine(Path, subPath));
        }
    }
}
