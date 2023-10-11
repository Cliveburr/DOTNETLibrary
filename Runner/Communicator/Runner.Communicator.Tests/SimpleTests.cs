using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Runner.Communicator.Process.FileUpload.Model;
using Runner.Communicator.Tests.Interfaces;
using Runner.Communicator.Tests.Model;
using Runner.Communicator.Tests.Services;

namespace Runner.Communicator.Tests
{
    [TestClass]
    public class SimpleTests
    {
        public (HostService, Server) StartServer()
        {
            Server? server = null;
            var host = new HostService()
                .ConfigureServices(services =>
                {
                    server = new Server(18810, services);
                    services
                        .AddSingleton(server);
                    server
                        .Add<IOnToTwoInterface, OnToTwoService>()
                        .Add<ITwoToOneInterface, TwoToOneService>();
                    server
                        .Start();
                })
                .Build();
            return (host, server!);
        }

        [TestMethod]
        public async Task PingPong()
        {
            var (host, server) = StartServer();
            await Task.Delay(100);

            using (var client = await Client.Connect("127.0.0.1", 18810))
            {
                var oneToTwoService = client.Open<IOnToTwoInterface>();
                var pong = await oneToTwoService.Ping();
                Assert.AreEqual(pong, "PONG");
            }

            server.Stop();
        }

        [TestMethod]
        public async Task PrimitiveParameters()
        {
            var (host, server) = StartServer();

            using (var client = await Client.Connect("127.0.0.1", 18810))
            {
                var oneToTwoService = client.Open<IOnToTwoInterface>();
                await oneToTwoService.PrimitiveParameters(true, "STRING", 123, short.MaxValue, long.MinValue);
            }

            server.Stop();
        }

        [TestMethod]
        public async Task ComplexModel()
        {
            var (host, server) = StartServer();

            using (var client = await Client.Connect("127.0.0.1", 18810))
            {
                var oneToTwoService = client.Open<IOnToTwoInterface>();

                var nowDatetime = new DateTime(10000);
                var backModel = await oneToTwoService.ComplexModel(new OneModel
                {
                    Id = 123,
                    Name = "OneModel",
                    NullStr = null,
                    NullTwoModel = null,
                    TwoModel = new TwoModel
                    {
                        Id = 234,
                        Active = true,
                        ThreeModels = new List<ThreeModel>
                        {
                            new ThreeModel { NowDateTime = nowDatetime },
                            new ThreeModel { NowDateTime = nowDatetime.AddDays(4) },
                            new ThreeModel { NowDateTime = nowDatetime.AddMilliseconds(333) }
                        }
                    }
                });

                Assert.AreEqual(backModel.Id, 321);
                Assert.AreEqual(backModel.Name, "Back");
                Assert.IsNull(backModel.NullStr);
                Assert.IsNull(backModel.NullTwoModel);
                Assert.IsNotNull(backModel.TwoModel, null);
                var two = backModel.TwoModel;
                Assert.AreEqual(two.Id, 432);
                Assert.IsFalse(two.Active);
                Assert.IsNotNull(two.ThreeModels);
                Assert.AreEqual(two.ThreeModels.Count, 2);
                var three = two.ThreeModels;
                var backDatetime = new DateTime(20000);
                Assert.AreEqual(three[0].NowDateTime, backDatetime);
                Assert.AreEqual(three[1].NowDateTime, backDatetime.AddMilliseconds(444));
            }

            server.Stop();
        }

        //[TestMethod]
        //public async Task TestTimeout()
        //{
        //    var (host, server) = StartServer();

        //    using (var client = await Client.Connect("127.0.0.1", 18810))
        //    {
        //        var oneToTwoService = client.Open<OnToTwoInterface>();

        //        client.TimeoutMilliseconds = 1000;
        //        try
        //        {
        //            await oneToTwoService.GetTimeout(5000);
        //            throw new Exception("Timeout fail!");
        //        }
        //        catch (Exception ex)
        //        {
        //            Assert.AreEqual(ex.GetType(), typeof(TimeoutException));
        //        }

        //        client.TimeoutMilliseconds = 5000;
        //        await oneToTwoService.GetTimeout(1000);
        //    }

        //    server.Stop();
        //}

        [TestMethod]
        public async Task TestException()
        {
            var (host, server) = StartServer();

            using (var client = await Client.Connect("127.0.0.1", 18810))
            {
                var twoToOneService = client.Open<ITwoToOneInterface>();

                try
                {
                    await twoToOneService.NormalException();
                    throw new Exception("Fail NormalException!");
                }
                catch (Exception err)
                {
                    Assert.IsTrue(err.Message.StartsWith("System.Exception: NormalException"));
                }

                try
                {
                    await twoToOneService.NullReferenceException();
                    throw new Exception("Fail NullReferenceException!");
                }
                catch (Exception err)
                {
                    Assert.IsTrue(err.Message.StartsWith("System.NullReferenceException: NullReferenceException"));
                }
            }

            server.Stop();
        }

        [TestMethod]
        public async Task Voidtask()
        {
            var (host, server) = StartServer();

            using (var client = await Client.Connect("127.0.0.1", 18810))
            {
                var oneToTwoService = client.Open<IOnToTwoInterface>();
                await oneToTwoService.Voidtask();
            }

            server.Stop();
        }
    }
}