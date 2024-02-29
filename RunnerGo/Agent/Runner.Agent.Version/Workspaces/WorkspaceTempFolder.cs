using Runner.Common.Helpers;
using Runner.Script.Interface.Scripts;

namespace Runner.Agent.Version.Workspaces
{
    public class WorkspaceTempFolder : WorkspaceFolder, IWorkspaceTempFolder, IDisposable
    {
        public WorkspaceTempFolder(string path)
            : base(path)
        {
        }

        public void Dispose()
        {
            IO.DeleteDirectory(Path);
        }
    }
}
