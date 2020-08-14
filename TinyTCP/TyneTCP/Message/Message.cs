using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using TyneTCP.Helpers;

namespace TyneTCP.Message
{
    /* Options
     * 0 = Is internal msg
     * 1 = Need confirmation
     * 2 = Is fragmented
     * 
     */

    internal class Message
    {
        public const int MESSAGEHEADERLENGTH = 1 + 1 + 2 + 2;

        public ushort Id { get; set; }

        /* Header */
        public Bitwise Options { get; set; }
        public MessageType Type { get; set; }
        public ushort PackagesCount { get; set; }

        /* Payload */
        public byte[] Payload { get; set; }

        public bool IsInternal
        {
            get 
            {
                return Options.GetBool(0);
            }
            set
            {
                Options.SetBool(0, value);
            }
        }

        public bool NeedConfirmation
        {
            get
            {
                return Options.GetBool(1);
            }
            set
            {
                Options.SetBool(1, value);
            }
        }

        public bool IsFragmented
        {
            get
            {
                return Options.GetBool(2);
            }
            set
            {
                Options.SetBool(2, value);
            }
        }

        public Package[] MakePackages(int packagesLength)
        {
            var totalPayloadLength = Payload.Length + MESSAGEHEADERLENGTH;
            Header.PackagesCount = (ushort)((totalPayloadLength / packagesLength) + 1);
            var packages = new Package[Header.PackagesCount];

            using (var memoryStream = new MemoryStream())
            using (var writeStream = new BinaryWriter(memoryStream))
            {
                SerializeHeader(writeStream);
                writeStream.Write(Payload);
                memoryStream.Position = 0;

                using (var readerStream = new BinaryReader(memoryStream))
                {
                    for (var index = 0; index < Header.PackagesCount; index++)
                    {
                        var packagePayload = readerStream.ReadBytes(packagesLength);
                        //packages[index] = new Package(Id, index, packagePayload);
                    }
                }
            }

            return packages;
        }
    }
}