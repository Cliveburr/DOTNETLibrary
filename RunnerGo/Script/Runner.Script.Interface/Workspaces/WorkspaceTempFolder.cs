
namespace Runner.Script.Interface.Workspaces
{
    public class WorkspaceTempFolder : WorkspaceFolder, IDisposable
    {
        public WorkspaceTempFolder(string path)
            : base(path)
        {
        }

        public void Dispose()
        {
            DeleteThisPath();
        }
    }
}
