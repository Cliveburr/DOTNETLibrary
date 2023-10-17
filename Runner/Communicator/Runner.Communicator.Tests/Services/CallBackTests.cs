using Microsoft.Extensions.DependencyInjection;
using Runner.Communicator.Helpers;
using Runner.Communicator.Process.Services2;
using Runner.Communicator.Tests.ServicesForTest.Implementation;
using Runner.Communicator.Tests.ServicesForTest.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Communicator.Tests.Services
{
    [TestClass]
    public class CallBackTests
    {
        public (BuildProvider, ServiceCallerImp, BuildProvider, ServiceCallerImp) Start()
        {
            var cancellationToken = new CancellationToken();

            var serverBuilder = BuildProvider.Build(services =>
            {
                services
                    .AddScoped<IClientToServer, ClientToServerService>()
                    .AddServiceCallerBack<ServiceCallerImp>();
            });

            var server = new ServiceCallerImp(serverBuilder.Provider.CreateScope(), cancellationToken);

            var clientBuilder = BuildProvider.Build(services =>
            {
                services
                    .AddScoped<IServerToClient, ServerToClientService>();
            });

            var client = new ServiceCallerImp(clientBuilder.Provider.CreateScope(), cancellationToken);

            server.ToCallInoker = client.CallInvokeAsync;
            client.ToCallInoker = server.CallInvokeAsync;

            return (serverBuilder, server, clientBuilder, client);
        }

        [TestMethod]
        public async Task JustCallBack()
        {
            var (serverBuilder, server, clientBuilder, client) = Start();

            var clientToServer = client.Open<IClientToServer>();
            var serverResponse = await clientToServer.ClientToServer();
            Assert.AreEqual(serverResponse, "SERVER");

            var serverToClient = server.Open<IServerToClient>();
            var clientResponse = await serverToClient.WaitServerCall();
            Assert.AreEqual(clientResponse, "CLIENT");
        }
    }
}
