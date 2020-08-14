using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TyneTCP.Message;

namespace TinyTCP.Test.Message
{
    [TestClass]
    public class PackageTest
    {

        [TestMethod]
        public void SimpleTest()
        {
            var rnd = new Random(DateTime.Now.Millisecond);

            var payloadLength = rnd.Next(1024);
            var payload = new byte[payloadLength];
            for (var i = 0; i < payloadLength; i++)
            {
                payload[i] = (byte)rnd.Next(256);
            }

            var packageSend = new Package(10, 23, payload);

            var packageBytes = packageSend.Serialize();

            var packageBack = new Package(packageBytes, payloadLength);

            Assert.AreEqual(packageSend.MessageId, packageBack.MessageId);
            Assert.AreEqual(packageSend.Index, packageBack.Index);
            Assert.AreEqual(packageSend.Checksum, packageBack.Checksum);
            ByteTestHelper.Compare(packageSend.Payload, packageBack.Payload);
        }
    }
}