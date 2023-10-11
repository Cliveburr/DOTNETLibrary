using Microsoft.Extensions.DependencyInjection;
using Runner.Communicator.Tests.Interfaces;
using Runner.Communicator.Tests.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Communicator.Tests
{
    [TestClass]
    public class BackCallTests
    {
        public (HostService, Server) StartServer()
        {
            Server? server = null;
            var host = new HostService()
                .ConfigureServices(services =>
                {
                    server = new Server(18810, services);
                    //services
                    //    .AddSingleton(server);
                    server
                        .Add<IOnToTwoInterface, OnToTwoService>()
                        .Add<ITwoToOneInterface, TwoToOneService>();
                    server
                        .Start();
                });
            return (host, server!);
        }


        [TestMethod]
        public async Task PingPong()
        {
            var (host, server) = StartServer();
            await Task.Delay(100);

            using (var client = await Client.Connect("127.0.0.1", 18810, host.Services))
            {
                client
                    .Add<IReverseInterface, ReverseService>();

                var oneToTwoService = client.Open<IOnToTwoInterface>();
                var pong = await oneToTwoService.Ping();
                Assert.AreEqual(pong, "PONG");
            }

            server.Stop();
        }
    }
}
