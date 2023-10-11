using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Runner.Communicator.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Runner.Communicator.Tests
{
    [TestClass]
    public class SocketTests
    {
        public byte[] GenerateData(int lengthMB)
        {
            var dataLength = lengthMB * (long)Math.Pow(2, 20);
            var data = new byte[dataLength];
            var random = new Random((int)DateTime.Now.Ticks);
            for (var i = 0; i < dataLength; i++)
            {
                data[i] = (byte)random.Next(0, 255);
            }
            return data;
        }

        public void CompareData(byte[] data1, byte[] data2)
        {
            Assert.AreEqual(data1.Length, data2.Length);
            for (var i = 0; i < data1.Length; i++)
            {
                Assert.AreEqual(data1[i], data2[i]);
            }
        }

        public byte[] ReverseData(byte[] data)
        {
            return data
                .Reverse()
                .ToArray();
        }

        [TestMethod]
        public async Task JustGo()
        {
            var wait = new ManualResetEvent(false);

            var lister = Task.Run(async () =>
            {
                var listener = new TcpListener(IPAddress.Any, 18900);
                listener.Start();

                var tcpClient = await listener.AcceptTcpClientAsync();

                var con = new SocketImpl(tcpClient, new Func<Message, Task<Message>>(message =>
                {
                    Assert.AreEqual(message.Data[0], 111);

                    return Task.FromResult(Message.Create(MessageType.Any, new byte[] { 222 }));
                }));
                con.Start(wait);
            });

            var conClient = new SocketImpl();
            await conClient.ConnectAsync("localhost", 18900);
            wait.WaitOne();
            await Task.Delay(100);

            var result = await conClient.SendAndReceive(Message.Create(MessageType.Any, new byte[] { 111 }));

            Assert.AreEqual(result.Data[0], 222);
        }

        [TestMethod]
        public async Task LargeData()
        {
            var wait = new ManualResetEvent(false);
            var data = GenerateData(500);

            var lister = Task.Run(async () =>
            {
                var listener = new TcpListener(IPAddress.Any, 18900);
                listener.Start();

                var tcpClient = await listener.AcceptTcpClientAsync();

                var con = new SocketImpl(tcpClient, new Func<Message, Task<Message>>(message =>
                {
                    CompareData(message.Data, data);

                    return Task.FromResult(Message.Create(MessageType.Any, new byte[] { 0 }));
                }));
                con.Start(wait);
            });

            var conClient = new SocketImpl();
            await conClient.ConnectAsync("localhost", 18900);
            wait.WaitOne();
            await Task.Delay(100);

            var result = await conClient.SendAndReceive(Message.Create(MessageType.Any, data));
        }

        [TestMethod]
        public async Task MultiplesClient()
        {
            var wait = new ManualResetEvent(false);
            var cancel = new CancellationTokenSource();

            var lister = Task.Run(async () =>
            {
                var listener = new TcpListener(IPAddress.Any, 18900);
                listener.Start();

                while (!cancel.Token.IsCancellationRequested)
                {
                    TcpClient? tcpClient = null;
                    try
                    {
                        tcpClient = await listener.AcceptTcpClientAsync(cancel.Token);
                    }
                    catch (Exception err)
                    {
                        Trace.TraceError(err.ToString());
                    }

                    if (tcpClient != null && !cancel.Token.IsCancellationRequested)
                    {
                        var con = new SocketImpl(tcpClient, new Func<Message, Task<Message>>(message =>
                        {
                            var reverseData = ReverseData(message.Data);

                            return Task.FromResult(Message.Create(MessageType.Any, reverseData));
                        }));
                        con.Start(wait);
                    }
                }

                listener.Stop();
            });

            var func = new Func<int, CancellationToken, ValueTask>(async (i, token) =>
            {
                var conClient = new SocketImpl();
                await conClient.ConnectAsync("localhost", 18900);

                var data = GenerateData(1);

                Trace.WriteLine($"client {i} start");
                var result = await conClient.SendAndReceive(Message.Create(MessageType.Any, data));

                var reverseData = ReverseData(data);
                CompareData(result.Data, reverseData);

                Trace.WriteLine($"client {i} close");
            });

            //var allClients = new List<Task>();
            //for (var i = 0; i < 1000; i++)
            //{
            //    var client = func(i);
            //    allClients.Add(client);
            //}
            var clientsList = Enumerable.Range(0, 100);


            await Parallel.ForEachAsync(clientsList, func);
            //Task.WaitAll(allClients.AsParallel().ToArray());

            cancel.Cancel();

            lister.Wait();
        }
    }
}

