using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TyneTCP.Helpers;

namespace TinyTCP.Test
{
    [TestClass]
    public class BinaryTesters
    {
        private byte[] ArrayTest0 = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
        private byte[] ArrayTest1 = new byte[] { 0, 1, 2, 3, 4 };
        private byte[] ArrayTest10 = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        [TestMethod]
        public void BucketInflate()
        {
            using (var binary = new BinaryBuffer(5))
            {
                binary.Write(ArrayTest0);

                binary.Position = 0;

                var back = binary.ReadBytes(12);

                ByteTestHelper.Compare(ArrayTest0, back);
            }
        }

        [TestMethod]
        public void OnEdgeOfBucket()
        {
            using (var binary = new BinaryBuffer(5))
            {
                var extraByte = new byte[] { 122 };

                binary.Write(ArrayTest1);
                binary.Write(extraByte);

                var fullInsert = ArrayTest1.Concat(extraByte).ToArray();
                binary.Position = 0;

                var back = binary.ReadBytes(6);

                ByteTestHelper.Compare(fullInsert, back);
            }
        }

        [TestMethod]
        public void BucketLargeInflate()
        {
            var array = new byte[10240];
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = (byte)(i % 255);
            }

            var sw = Stopwatch.StartNew();

            using (var binary = new BinaryBuffer(1024))
            {
                binary.Write(array);

                binary.Position = 0;

                var back = binary.ReadBytes(array.Length);

                sw.Stop();

                ByteTestHelper.Compare(array, back);
            }

            Debug.WriteLine($"BinaryTesters.BucketLargeInflate - Bytes: {array.Length.ToString()} - Elapsed: {sw.Elapsed.ToString()}");
        }

        [TestMethod]
        public void DNETBucketLargeInflate()
        {
            var array = new byte[10240];
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = (byte)(i % 255);
            }

            var sw = Stopwatch.StartNew();

            using (var memoryStream = new MemoryStream())
            using (var writeStream = new BinaryWriter(memoryStream))
            {
                writeStream.Write(array);

                memoryStream.Position = 0;

                var back = memoryStream.ToArray();

                sw.Stop();

                ByteTestHelper.Compare(array, back);
            }

            Debug.WriteLine($"BinaryTesters.DNETBucketLargeInflate - Bytes: {array.Length.ToString()} - Elapsed: {sw.Elapsed.ToString()}");
        }

        [TestMethod]
        public void RandomBucketInflate()
        {
            var rnd = new Random(DateTime.Now.Millisecond);

            var totalArrays = 200;
            var totalBytes = 0;
            var arrays = new byte[totalArrays][];
            for (var i = 0; i < arrays.Length; i++)
            {
                var count = rnd.Next(0, 1024);
                totalBytes += count;
                var thisArray = new byte[count];
                for (var o = 0; o < count; o++)
                {
                    thisArray[o] = (byte)(o % 255);
                }
                arrays[i] = thisArray;
            }

            var sw = Stopwatch.StartNew();

            using (var binary = new BinaryBuffer())
            {
                for (var i = 0; i < arrays.Length; i++)
                {
                    binary.Write(arrays[i]);
                }

                binary.Position = 0;

                var backs = new byte[totalArrays][];
                for (var i = 0; i < arrays.Length; i++)
                {
                    var back = binary.ReadBytes(arrays[i].Length);
                    backs[i] = back;
                }

                sw.Stop();

                for (var i = 0; i < arrays.Length; i++)
                {
                    ByteTestHelper.Compare(arrays[i], backs[i]);
                }
            }

            Debug.WriteLine($"BinaryTesters.RandomBucketInflate - Bytes: {totalBytes.ToString()} - Elapsed: {sw.Elapsed.ToString()}");

            var sw2 = Stopwatch.StartNew();

            using (var memoryStream = new MemoryStream())
            using (var writeStream = new BinaryWriter(memoryStream))
            {
                for (var i = 0; i < arrays.Length; i++)
                {
                    writeStream.Write(arrays[i]);
                }

                memoryStream.Position = 0;

                using (var readStream = new BinaryReader(memoryStream))
                {
                    var backs = new byte[totalArrays][];
                    for (var i = 0; i < arrays.Length; i++)
                    {
                        var back = readStream.ReadBytes(arrays[i].Length);
                        backs[i] = back;
                    }

                    sw2.Stop();

                    for (var i = 0; i < arrays.Length; i++)
                    {
                        ByteTestHelper.Compare(arrays[i], backs[i]);
                    }
                }
            }

            Debug.WriteLine($"BinaryTesters.DNETRandomBucketInflate - Bytes: {totalBytes.ToString()} - Elapsed: {sw2.Elapsed.ToString()}");
        }

        [TestMethod]
        public void Buffer2Size()
        {
            using (var binary = new BinaryBuffer(10))
            {
                binary.Write(ArrayTest10);
                binary.Write(new byte[] { 66 });

                binary.Position = 0;

                var back = binary.ReadBytes(10);

                ByteTestHelper.Compare(ArrayTest10, back);
            }
        }
    }
}
