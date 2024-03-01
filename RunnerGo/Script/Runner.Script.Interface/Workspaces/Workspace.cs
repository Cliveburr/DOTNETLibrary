using Runner.Script.Interface.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Script.Interface.Workspaces
{
    public class Workspace
    {
        public string BasePath { get; init; }

        public Workspace(string flowId)
        {
            BasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "work", flowId);
        }

        public WorkspaceFolder GetFolder(string name)
        {
            return new WorkspaceFolder(Path.Combine(BasePath, name));
        }

        public WorkspaceTempFolder GetTemp()
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
