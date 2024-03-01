using Build.Scripts.Nodejs;
using Runner.Script.Interface.Model.Data;
using Runner.Script.Interface.Scripts;
using Runner.Script.Interface.Workspaces;

namespace Build.Scripts.Tests
{
    [TestClass]
    public class NodejsTests : TestsBase
    {
        [TestMethod]
        public async Task NpmInstall()
        {
            var context = new ScriptRunContext
            {
                SiteUrl = "https://localhost:8080/app/newtest/flow1",
                Data = new ScriptData([
                    StringProperty("WorkingFolder", "WebUI"),
                    StringProperty("Command", "install"),
                ]),
                CancellationToken = CancellationToken.None,
                Log = WriteLog,
                Workspace = new Workspace(@"D:\testclone")
            };

            var script = new NpmCommand();
            await script.Run(context);
        }

        [TestMethod]
        public async Task NpmBuild()
        {
            var context = new ScriptRunContext
            {
                SiteUrl = "https://localhost:8080/app/newtest/flow1",
                Data = new ScriptData([
                    StringProperty("WorkingFolder", "WebUI"),
                    StringProperty("Command", "run build"),
                ]),
                CancellationToken = CancellationToken.None,
                Log = WriteLog,
                Workspace = new Workspace(@"D:\testclone")
            };

            var script = new NpmCommand();
            await script.Run(context);
        }
    }
}
