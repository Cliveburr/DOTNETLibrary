using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using TyneTCP.Helpers;

namespace TyneTCP.Message
{
    internal class Package
    {
        public const int PACKAGEHEADERLENGTH = 3;

        /* Header */
        public ushort MessageId { get; private set; }
        public byte Index { get; private set; }
        public ushort Checksum { get; private set; }

        /* Payload */
        public byte[] Payload { get; private set; }

        public Package(ushort messageId, byte index, byte[] payload)
        {
            /* Header */
            MessageId = messageId;
            Index = index;
            Checksum = Helpers.Checksum.CalculateChecksum(payload);

            /* Payload */
            Payload = payload;
        }

        public Package(byte[] buffer, int length)
        {
            using (var memoryStream = new MemoryStream(buffer))
            using (var readerStream = new BinaryReader(memoryStream))
            {
                /* Header */
                MessageId = readerStream.ReadUInt16();
                Index = readerStream.ReadByte();
                Checksum = readerStream.ReadUInt16();

                /* Payload */
                Payload = readerStream.ReadBytes((int)(length - memoryStream.Position));
            }

            var checksum = Helpers.Checksum.CalculateChecksum(Payload);
            if (Checksum != checksum)
            {
                throw new Exception("Package checksum fail!");
            }
        }

        public byte[] Serialize()
        {
            using (var memoryStream = new MemoryStream())
            using (var writeStream = new BinaryWriter(memoryStream))
            {
                /* Header */
                writeStream.Write(MessageId);
                writeStream.Write(Index);
                writeStream.Write(Checksum);

                /* Payload */
                if (Payload != null)
                {
                    writeStream.Write(Payload);
                }

                return memoryStream.ToArray();
            }
        }
    }
}