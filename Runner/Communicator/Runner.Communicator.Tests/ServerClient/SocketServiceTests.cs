using Microsoft.Extensions.DependencyInjection;
using Runner.Communicator.Tests.Services;
using Runner.Communicator.Tests.ServicesForTest.Implementation;
using Runner.Communicator.Tests.ServicesForTest.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Communicator.Tests.ServerClient
{
    [TestClass]
    public class SocketServiceTests
    {
        public (BuildProvider, Server, BuildProvider) Start()
        {
            var serverBuilder = BuildProvider.Build(services =>
            {
                services
                    .AddScoped<IBasic, BasicService>();
            });

            var server = new Server(18810, serverBuilder.Provider);
            server.Start();

            var clientBuilder = BuildProvider.Build(services =>
            {
            });

            return (serverBuilder, server, clientBuilder);
        }

        [TestMethod]
        public async Task PingPong()
        {
            var (serverBuilder, server, clientBuilder) = Start();

            using (var client = await Client.Connect("127.0.0.1", 18810, clientBuilder.Provider.CreateScope()))
            {
                var basic = client.Services.Open<IBasic>();
                var pong = await basic.Ping();
                Assert.AreEqual(pong, "PONG");
            }
        }
    }
}
