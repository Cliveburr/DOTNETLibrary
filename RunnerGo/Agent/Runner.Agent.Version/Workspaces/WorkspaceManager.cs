using Runner.Script.Interface.Scripts;

namespace Runner.Agent.Version.Workspaces
{
    public class WorkspaceManager : IWorkspace
    {
        public string BasePath { get; init; }

        public WorkspaceManager(string flowId)
        {
            BasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "work", flowId);
        }

        public IWorkspaceFolder GetFolder(string name)
        {
            return new WorkspaceFolder(Path.Combine(BasePath, name));
        }

        public IWorkspaceTempFolder GetTemp()
        {
            var iteration = 0;

            var path = "";
            do
            {
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp", (iteration++).ToString());
            } while (Directory.Exists(path));

            Directory.CreateDirectory(path);

            return new WorkspaceTempFolder(path);
        }
    }
}
