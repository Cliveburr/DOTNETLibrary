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
            TcpListener? listener = null;

            var lister = Task.Run(async () =>
            {
                listener = new TcpListener(IPAddress.Any, 18900);
                listener.Start();

                var tcpClient = await listener.AcceptTcpClientAsync();

                var con = new SocketImpl(tcpClient, (data, port) =>
                {
                    Assert.AreEqual(data[0], 111);

                    return Task.FromResult<byte[]?>(new byte[] { 222 });
                });
                con.StartReceive();
                wait.Set();
            });

            var conClient = new SocketImpl();
            await conClient.ConnectAsync("localhost", 18900);
            wait.WaitOne();

            var result = await conClient.SendAndReceive(new byte[] { 111 }, MessagePort.Any);

            Assert.AreEqual(result[0], 222);
            listener?.Stop();
            lister.Dispose();
        }

        [TestMethod]
        public async Task LargeData()
        {
            var wait = new ManualResetEvent(false);
            TcpListener? listener = null;
            var data = GenerateData(500);

            var lister = Task.Run(async () =>
            {
                listener = new TcpListener(IPAddress.Any, 18900);
                listener.Start();

                var tcpClient = await listener.AcceptTcpClientAsync();

                var con = new SocketImpl(tcpClient, (data, port) =>
                {
                    CompareData(data, data);

                    return Task.FromResult<byte[]?>(new byte[] { 0 });
                });
                con.StartReceive();
                wait.Set();
            });

            var conClient = new SocketImpl();
            await conClient.ConnectAsync("localhost", 18900);
            wait.WaitOne();

            var result = await conClient.SendAndReceive(data, MessagePort.Any);
            listener?.Stop();
            lister.Dispose();
        }

        [TestMethod]
        public async Task MultiplesClient()
        {
            var wait = new ManualResetEvent(false);
            var cancel = new CancellationTokenSource();
            TcpListener? listener = null;

            var lister = Task.Run(async () =>
            {
                listener = new TcpListener(IPAddress.Any, 18900);
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
                        var con = new SocketImpl(tcpClient, (data, port) =>
                        {
                            var reverseData = ReverseData(data);

                            return Task.FromResult<byte[]?>(reverseData);
                        });
                        con.StartReceive();
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
                var result = await conClient.SendAndReceive(data, MessagePort.Any);

                var reverseData = ReverseData(data);
                CompareData(result, reverseData);

                Trace.WriteLine($"client {i} close");
            });

            var clientsList = Enumerable.Range(0, 100);
            await Parallel.ForEachAsync(clientsList, func);

            cancel.Cancel();
            lister.Wait();

            listener?.Stop();
            lister.Dispose();
        }
    }
}

