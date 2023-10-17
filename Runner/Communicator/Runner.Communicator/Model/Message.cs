using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Runner.Communicator.Helpers;

namespace Runner.Communicator.Model
{
    internal class Message
    {
        public MessageHead Head { get; private set; }
        public byte[] Data { get; private set; }

        public Message(byte[] data, ulong id, MessagePort port, bool isResponse, bool isSuccess)
        {
            Head = new MessageHead
            {
                Id = id,
                Port = port,
                IsResponse = isResponse,
                IsSuccess = isSuccess,
                DataLenght = (uint)data.Length
            };
            Data = data;
        }

        public Message(MessageHead head, byte[] data)
        {
            Head = head;
            Data = data;
        }
    }

    internal class MessageHead
    {
        public static int HeadLenght { get; } = 5;

        public ulong Id { get; set; }
        public MessagePort Port { get; set; }
        public bool IsResponse { get; set; }
        public bool IsSuccess { get; set; }
        public uint DataLenght { get; set; }

        public static MessageHead Parse(byte[] buffer)
        {
            var reader = new BytesReader(buffer);
            var type = (MessagePort)reader.ReadByte();
            var lenght = reader.ReadUInt32();

            return new MessageHead
            {
                Type = type,
                DataLenght = lenght
            };
        }

        public byte[] GetBytes()
        {
            var writer = new BytesWriter()
                .WriteByte((byte)Type)
                .WriteUInt32(DataLenght);

            return writer
                .GetBytes();
        }
    }
}
