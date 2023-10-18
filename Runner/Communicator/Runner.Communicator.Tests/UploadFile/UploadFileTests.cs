//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Runner.Communicator.FileUpload;
//using Runner.Communicator.Model;
//using Runner.Communicator.Tests.Interfaces;
//using Runner.Communicator.Tests.Services;

//namespace Runner.Communicator.Tests.UploadFile
//{
//    [TestClass]
//    public class UploadFileTests
//    {
//        public (HostService, Server) StartServer()
//        {
//            Server? server = null;
//            var host = new HostService()
//                .ConfigureServices(services =>
//                {
//                    server = new Server(18810, services);
//                    server.FileUploadDirectory = @"D:\UploadTests";
//                    server.OnError += Server_OnError;
//                    services
//                        .AddSingleton(server);
//                    server
//                        .Add<ITwoToOneInterface, TwoToOneService>()
//                        .Start();
//                })
//                .Build();
//            return (host, server!);
//        }

//        private void Server_OnError(Server server, ServerConnection? connection, Exception err)
//        {
//            Trace.TraceError(err.ToString());
//        }

//        [TestMethod]
//        public async Task Upload()
//        {
//            var (host, server) = StartServer();

//            using (var client = await Client.Connect("127.0.0.1", 18810))
//            {
//                //client.TimeoutMilliseconds = 180000;

//                var localFilePath = @"D:\UploadTests\From\Test.7z";
//                var destineFilePath = @"D:\UploadTests\To\Test.7z";

//                var fileClient = client.FileUpload(localFilePath, destineFilePath, true);

//                await fileClient.Upload(3, (log) => Trace.WriteLine(log));

//                var twoToOne = client.Open<ITwoToOneInterface>();
//                await twoToOne.Extracheck(destineFilePath);
//            }

//            server.Stop();
//        }

//        [TestMethod]
//        public async Task DeleteFolder()
//        {
//            var (host, server) = StartServer();

//            //using (var client = await Client.Connect("127.0.0.1", 18810))
//            using (var client = await Client.Connect("192.168.237.29", 6677))
//            {
//                //client.TimeoutMilliseconds = 180000;

//                var folder = "delete";

//                await client.DeleteFolder(folder);
//            }

//            server.Stop();
//        }
//    }
//}
