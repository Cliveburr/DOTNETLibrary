using Build.Scripts.Azure;

namespace Build.Scripts.Tests
{
    [TestClass]
    public class AzureTests
    {
        private string _uri = "https://dev.azure.com/ttibrazil/";
        private string _token = "srybl3appa3o5w63dxqdamy6iat2enx4dj2u7vbvlwqxnc62vqba";
        private string _path = @"D:\testclone\s";

        [TestMethod]
        public async Task GetRepository()
        {
            using (var azureAccess = new AzureAccess(_uri, _token))
            {
                await azureAccess.Connect();

                await azureAccess.GetRepository("Internal", "ProjectSample", "test1", _path);
            }
        }

        [TestMethod]
        public async Task CreateBuildStatus()
        {
            using (var azureAccess = new AzureAccess(_uri, _token))
            {
                await azureAccess.Connect();

                await azureAccess.CreateBuildStatus("Internal", "ProjectSample", _path);
            }
        }
    }
}