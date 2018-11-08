using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hashing
{
    class Program
    {
        static void Main(string[] args)
        {

            //TestSHA256Other();

            TestSHA256Another();

        }

        private static void TestHashs(byte[] dotnet, byte[] custom)
        {
            if (dotnet.Length != custom.Length)
                throw new Exception();

            for (var i = 0; i < custom.Length; i++)
            {
                if (dotnet[i] != custom[i])
                    throw new Exception();
            }

            var sdotnet = BitConverter.ToString(dotnet).Replace("-", "");
            var scustom = BitConverter.ToString(custom).Replace("-", "");
        }

        private static void TestSHAOne()
        {
            var testhas1 = new SHAOne("A Test");

            var dotnet = testhas1.NetFunction();

            var custom = testhas1.CustomFunction();

            TestHashs(dotnet, custom);
        }

        private static void TestSHA256Other()
        {
            var testsha256text = "Neque porro quisquam est qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit";
            var testsha256textBytes = Encoding.UTF8.GetBytes(testsha256text);

            using (var sha256 = new System.Security.Cryptography.SHA256Managed())
            {
                var dotnet = sha256.ComputeHash(testsha256textBytes);

                var other = Sha2.Sha256.HashFile(testsha256textBytes).ToArray();

                TestHashs(dotnet, other);
            }
        }

        private static void TestSHA256Another()
        {
            var testsha256text = "Neque porro quisquam est qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit";
            var testsha256textBytes = Encoding.UTF8.GetBytes(testsha256text);

            using (var sha256 = new System.Security.Cryptography.SHA256Managed())
            {
                var dotnet = sha256.ComputeHash(testsha256textBytes);

                var another = SHA256Another.SHA256(testsha256textBytes);

                TestHashs(dotnet, another);
            }
        }
    }
}
