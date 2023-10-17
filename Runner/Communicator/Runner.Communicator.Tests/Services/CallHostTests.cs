using Microsoft.Extensions.DependencyInjection;
using Runner.Communicator.Tests.Model;
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
    public class CallHostTests
    {
        public (BuildProvider, ServiceCallerImp, BuildProvider, ServiceCallerImp) Start()
        {
            var cancellationToken = new CancellationToken();

            var hostBuilder = BuildProvider.Build(services =>
            {
                services
                    .AddScoped<IBasic, BasicService>();
            });

            var host = new ServiceCallerImp(hostBuilder.Provider.CreateScope(), cancellationToken);

            var callBuilder = BuildProvider.Build(services =>
            {
            });

            var call = new ServiceCallerImp(callBuilder.Provider.CreateScope(), cancellationToken);

            host.ToCallInoker = call.CallInvokeAsync;
            call.ToCallInoker = host.CallInvokeAsync;

            return (hostBuilder, host, callBuilder, call);
        }

        [TestMethod]
        public async Task PingPong()
        {
            var (hostBuilder, host, callBuilder, call) = Start();
            
            var basic = call.Open<IBasic>();
            var pong = await basic.Ping();
            Assert.AreEqual(pong, "PONG");
        }

        [TestMethod]
        public async Task PrimitiveParameters()
        {
            var (hostBuilder, host, callBuilder, call) = Start();

            var basic = call.Open<IBasic>();
            await basic.PrimitiveParameters(true, "STRING", 123, short.MaxValue, long.MinValue);
        }

        [TestMethod]
        public async Task ComplexModel()
        {
            var (hostBuilder, host, callBuilder, call) = Start();

            var basic = call.Open<IBasic>();

            var nowDatetime = new DateTime(10000);
            var backModel = await basic.ComplexModel(new OneModel
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
            var (hostBuilder, host, callBuilder, call) = Start();

            var basic = call.Open<IBasic>();

            try
            {
                await basic.NormalException();
                throw new Exception("Fail NormalException!");
            }
            catch (Exception err)
            {
                Assert.IsTrue(err.Message.StartsWith("System.Exception: NormalException"));
            }

            try
            {
                await basic.NullReferenceException();
                throw new Exception("Fail NullReferenceException!");
            }
            catch (Exception err)
            {
                Assert.IsTrue(err.Message.StartsWith("System.NullReferenceException: NullReferenceException"));
            }
        }

        [TestMethod]
        public async Task Voidtask()
        {
            var (hostBuilder, host, callBuilder, call) = Start();

            var basic = call.Open<IBasic>();

            await basic.Voidtask();
        }
    }
}
