using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace TyneTCP.Helpers
{
    public static class Checksum
    {
        public static ushort CalculateChecksum(byte[] bytes)
        {
            using (var md5 = MD5.Create())
            {
                if (bytes != null)
                {
                    return BitConverter.ToUInt16(md5.ComputeHash(bytes));
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}