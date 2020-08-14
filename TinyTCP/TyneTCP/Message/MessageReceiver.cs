using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TyneTCP.Message
{
    internal class MessageReceiver
    {
        public Message Message { get; private set; }
        
        private List<Package> _packages;
        private int _firstPackagePayloadStart;

        public MessageReceiver(Package package)
        {
            _packages = new List<Package>
            {
                package
            };

            Message = new Message
            {
                Id = package.MessageId
            };

            DeserializeMessageHeader(package);
        }

        private void DeserializeMessageHeader(Package package)
        {
            using (var memoryStream = new MemoryStream(package.Payload))
            using (var readerStream = new BinaryReader(memoryStream))
            {
                Message.Options = new Helpers.Bitwise(readerStream.ReadByte());

                if (Message.IsInternal)
                {
                    Message.Type = (MessageType)readerStream.ReadByte();
                }
                else
                {
                    Message.Type = MessageType.NotInternal;
                }

                if (Message.IsFragmented)
                {
                    Message.PackagesCount = readerStream.ReadUInt16();
                }
                else
                {
                    Message.PackagesCount = 1;
                }

                _firstPackagePayloadStart = (int)memoryStream.Position;
            }
        }

        public void AddPackage(Package package)
        {
            if (_packages.Any(p => p.Index == package.Index))
            {
                return;
            }

            _packages.Add(package);
        }

        public bool IsFinished()
        {
            return _packages.Count == Message.PackagesCount;
        }

        public void ComputatePayload()
        {
            using (var memoryStream = new MemoryStream())
            using (var writeStream = new BinaryWriter(memoryStream))
            {
                var firstPackage = _packages[0];
                writeStream.Write(firstPackage.Payload, _firstPackagePayloadStart, firstPackage.Payload.Length - _firstPackagePayloadStart);

                for (var i = 1; i < _packages.Count; i++)
                {
                    writeStream.Write(_packages[i].Payload);
                }

                Message.Payload = memoryStream.ToArray();
            }
        }
    }
}