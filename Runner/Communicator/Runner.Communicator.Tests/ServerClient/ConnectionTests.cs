//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Collections.Generic;
//using System.Dynamic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Runner.Communicator.Tests.Interfaces;
//using Runner.Communicator.Tests.Services;

//namespace Runner.Communicator.Tests
//{
//    [TestClass]
//    public class ConnectionTests
//    {
//        public (HostService, Server) StartServer()
//        {
//            Server? server = null;
//            var host = new HostService()
//                .ConfigureServices(services =>
//                {
//                    server = new Server(18810, services);
//                    services
//                        .AddSingleton(server);
//                    server
//                        .Add<IOnToTwoInterface, OnToTwoService>()
//                        .Add<IWaiterInterface, WaiterService>();
//                    server
//                        .Start();
//                })
//                .Build();
//            return (host, server!);
//        }

//        //[TestMethod]
//        //public void MultipleClients()
//        //{
//        //    var (host, server) = StartServer();

//        //    var count = 50;
//        //    var clients = new Client[count];
//        //    var waitAllConnect = new Task[count];
//        //    for (var i = 0; i < count; i++)
//        //    {
//        //        clients[i] = new Client("127.0.0.1", 18810);
//        //        waitAllConnect[i] = clients[i].ConnectAsync();
//        //    }

//        //    Task.WaitAll(waitAllConnect);

//        //    var waitMessage = new Task[count];
//        //    var rets = new bool[count];
//        //    for (var i = 0; i < count; i++)
//        //    {
//        //        var waiterService = clients[i].Open<IWaiterInterface>();
//        //        var thisI = i;
//        //        waitMessage[thisI] = Task.Run(async () =>
//        //        {
//        //            rets[thisI] = false;
//        //            var tret = await waiterService.Wait(1);
//        //            rets[thisI] = true;
//        //        });
//        //    }

//        //    Assert.AreEqual(server.Connections.Count, count);

//        //    Task.WaitAll(waitMessage);

//        //    Assert.IsFalse(rets.Any(r => !r));

//        //    for (var i = 0; i < count; i++)
//        //    {
//        //        clients[i].Dispose();
//        //    }

//        //    Thread.Sleep(100);

//        //    Assert.AreEqual(server.Connections.Count, 0);

//        //    server.Stop();
//        //}

//        [TestMethod]
//        public async Task DisconectClient()
//        {
//            var (host, server) = StartServer();

//            using (var client = await Client.Connect("127.0.0.1", 18810))
//            {
//                var oneToTwoService = client.Open<IOnToTwoInterface>();
//                var pong = await oneToTwoService.Ping();
//                Assert.AreEqual(pong, "PONG");
//            }

//            // precisa passar timeout manual para 1000 para testar
//            Thread.Sleep(10000);

//            Assert.AreEqual(server.Connections.Count, 0);

//            server.Stop();
//        }

//        [TestMethod]
//        public async Task DisconectServer()
//        {
//            var (host, server) = StartServer();

//            using (var client = await Client.Connect("127.0.0.1", 18810))
//            {
//                //client.TimeoutMilliseconds = 50000;

//                var oneToTwoService = client.Open<IOnToTwoInterface>();
//                var task = oneToTwoService.GetTimeout(1000);

//                try
//                {
//                    server.Stop();

//                    await task.WaitAsync(TimeSpan.FromHours(10));
//                    throw new Exception("Server closed fail!");
//                }
//                catch (Exception err)
//                {
//                    Assert.IsTrue(err.GetType().Equals(typeof(EndOfStreamException)));
//                }

//                try
//                {
//                    await oneToTwoService.GetTimeout(1000);
//                    throw new Exception("Server closed fail2!");
//                }
//                catch (Exception err)
//                {
//                    Assert.IsTrue(err.GetType().Equals(typeof(InvalidOperationException)));
//                }
//            }

//        }
//    }
//}
