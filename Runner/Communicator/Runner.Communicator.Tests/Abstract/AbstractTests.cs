using Runner.Communicator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Communicator.Tests.Abstract
{
    [TestClass]
    public class AbstractTests
    {
        [TestMethod]
        public async Task SimpleConnect()
        {
            var isConnected = false;

            var socket = new BaseImpl(3000);
            socket.OnConnected = (cancellationToken) =>
            {
                isConnected = true;
                cancellationToken.ThrowIfCancellationRequested();
                return true;
            };

            await socket.ConnectAsync();

            Assert.IsTrue(isConnected);
        }

        [TestMethod]
        public async Task FailConnect()
        {
            var socket = new BaseImpl(0);
            socket.OnConnected = (cancellationToken) =>
            {
                throw new ArgumentOutOfRangeException();
            };

            try
            {
                await socket.ConnectAsync();
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.IsTrue(false);
        }

        [TestMethod]
        public async Task TimeoutConnect()
        {
            var socket = new BaseImpl(10);
            socket.OnConnected = (cancellationToken) =>
            {
                Task.WaitAny(new Task[]
                {
                    Task.Delay(1000)
                },
                    cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                return false;
            };

            try
            {
                await socket.ConnectAsync();
            }
            catch (OperationCanceledException ex)
            {
                Assert.IsTrue(true);
                return;
            }

            Assert.IsTrue(false);
        }

        [TestMethod]
        public async Task SimpleSend()
        {
            var isConnected = false;

            var socket = new BaseImpl(3000);
            socket.OnConnected = (cancellationToken) =>
            {
                isConnected = true;
                return true;
            };

            var dataSend = new byte[] { 10, 11, 12 };
            await socket.SendMessage(new Message(MessagePort.Any, dataSend));
            var dataReceive = await socket.ReceiveMessage();

            Assert.IsTrue(isConnected);
            Assert.IsNotNull(dataReceive);
            CompareData(dataSend, dataReceive.Data);
        }

        public void CompareData(byte[] data1, byte[] data2)
        {
            Assert.AreEqual(data1.Length, data2.Length);
            for (var i = 0; i < data1.Length; i++)
            {
                Assert.AreEqual(data1[i], data2[i]);
            }
        }
    }
}
