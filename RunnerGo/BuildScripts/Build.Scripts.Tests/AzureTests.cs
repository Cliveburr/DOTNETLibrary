using Build.Scripts.Azure;
using Build.Scripts.Tests.Configuration;

namespace Build.Scripts.Tests
{
    [TestClass]
    public class AzureTests
    {
        private string _path = @"D:\testclone\s";

        [TestMethod]
        public async Task GetRepository()
        {
            var (uri, token) = ReadConfiguraiton.ReadUriAndToken();

            using (var azureAccess = new AzureAccess(uri, token))
            {
                await azureAccess.Connect();

                await azureAccess.GetRepository("Internal", "ProjectSample", "test1", _path);
            }
        }

        [TestMethod]
        public async Task CreateBuildStatus()
        {
            var (uri, token) = ReadConfiguraiton.ReadUriAndToken();

            using (var azureAccess = new AzureAccess(uri, token))
            {
                await azureAccess.Connect();

                await azureAccess.CreateCommitStatus("Internal", "ProjectSample", _path, true, "Description test", "Runner", "app/testnew/flow1", "https://localhost:1010/app/testnew/flow1");
            }
        }
    }
}