using Build.Scripts.Nuget;
using Runner.Script.Interface.Model.Data;
using Runner.Script.Interface.Scripts;
using Runner.Script.Interface.Workspaces;

namespace Build.Scripts.Tests
{
    [TestClass]
    public class NugetTests : TestsBase
    {
        [TestMethod]
        public async Task NugetRestore()
        {
            var context = new ScriptRunContext
            {
                SiteUrl = "https://localhost:8080/app/newtest/flow1",
                Data = new ScriptData([
                    StringProperty("SolutionFile", "WebAPI\\WebAPI.sln"),
                ]),
                CancellationToken = CancellationToken.None,
                Log = WriteLog,
                Workspace = new Workspace(@"D:\testclone")
            };

            var script = new NugetRestore();
            await script.Run(context);
        }
    }
}
