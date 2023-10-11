using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Runner.Communicator.Helpers;

namespace Runner.Communicator.Model
{
    public class Message
    {
        public MessageHead Head { get; private set; }
        public byte[] Data { get; private set; }

        public Message(MessageHead head, byte[] data)
        {
            Head = head;
            Data = data;
        }

        public Message(MessageType type, byte[] data)
        {
            Head = new MessageHead
            {
                Type = type,
                Lenght = (uint)data.Length
            };
            Data = data;
        }

        public static Message Create(MessageType type, byte[] data)
        {
            return new Message(type, data);
        }
    }

    public class MessageHead
    {
        public static int HeadLenght { get; } = 5;

        public MessageType Type { get; set; }
        public uint Lenght { get; set; }

        public static MessageHead Parse(byte[] buffer)
        {
            var reader = new BytesReader(buffer);
            var type = (MessageType)reader.ReadByte();
            var lenght = reader.ReadUInt32();

            return new MessageHead
            {
                Type = type,
                Lenght = lenght
            };
        }

        public byte[] GetBytes()
        {
            var writer = new BytesWriter()
                .WriteByte((byte)Type)
                .WriteUInt32(Lenght);

            return writer
                .GetBytes();
        }
    }
}
