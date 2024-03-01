using Build.Scripts.MSBuild;
using Runner.Script.Interface.Model.Data;
using Runner.Script.Interface.Scripts;
using Runner.Script.Interface.Workspaces;

namespace Build.Scripts.Tests
{
    [TestClass]
    public class MSBuildTests : TestsBase
    {
        [TestMethod]
        public async Task MSBuildBuild()
        {
            var context = new ScriptRunContext
            {
                SiteUrl = "https://localhost:8080/app/newtest/flow1",
                Data = new ScriptData([
                    StringProperty("Project", "WebAPI\\WebAPI.sln"),
                    StringProperty("Platform", "any cpu"),
                    StringProperty("Configuration", "release"),
                ]),
                CancellationToken = CancellationToken.None,
                Log = WriteLog,
                Workspace = new Workspace(@"D:\testclone")
            };

            var script = new MSBuildCommand();
            await script.Run(context);
        }
    }
}
