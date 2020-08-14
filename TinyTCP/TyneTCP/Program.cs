using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using TyneTCP.Helpers;

namespace TyneTCP
{

    /*
     * client
     * server
     * Channels
     * Reliability
     * Fragmentation
     * 
     * 
     */

    class Program
    {
        static void Main(string[] args)
        {
            TestPerf();
            return;

            Console.WriteLine("Hello World!");

            var bits = new Bitwise(0);

            var t = bits.GetBool(0);
            bits.SetBool(0, true);
            bits.SetBool(7, true);
            bits.SetBool(4, true);
            var t2 = bits.GetBool(0);
            var t21 = bits.GetBool(1);
            var t22 = bits.GetBool(2);
            var t23 = bits.GetBool(4);
            var t24 = bits.GetBool(7);
            bits.SetBool(0, false);
            var t3 = bits.GetBool(0);

            //var udp = new Socket(SocketType.Dgram, ProtocolType.Udp);

            //udp.SendAsync()
        }

        static void TestPerf()
        {
            var result = new StringBuilder();
            var rnd = new Random(DateTime.Now.Millisecond);

            var totalArrays = 3000;
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
            }

            result.AppendLine($"BinaryTesters.RandomBucketInflate - Bytes: {totalBytes.ToString()} - Elapsed: {sw.Elapsed.ToString()}");

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
                }
            }

            result.AppendLine($"BinaryTesters.DNETRandomBucketInflate - Bytes: {totalBytes.ToString()} - Elapsed: {sw2.Elapsed.ToString()}");

            File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "perf.txt"), result.ToString());
        }
    }
}

