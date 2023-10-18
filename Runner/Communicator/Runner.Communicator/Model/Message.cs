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

        public Message(byte[] data, ushort id, MessagePort port, bool isResponse, bool isSuccess)
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
        // id ushort = 2 bytes
        // port = 1 byte
        // bitFields = 1 byte
        // dataLenght = 4 byte
        public static int HeadLenght { get; } = 8;

        public ushort Id { get; set; }
        public MessagePort Port { get; set; }
        public bool IsResponse { get; set; }
        public bool IsSuccess { get; set; }
        public uint DataLenght { get; set; }

        public static MessageHead Parse(byte[] buffer)
        {
            var reader = new BytesReader(buffer);
            var id = reader.ReadUInt16();
            var port = (MessagePort)reader.ReadByte();
            var bitFields = reader.ReadByte();
            var lenght = reader.ReadUInt32();

            var isResponse = BitFields.ReadBool(bitFields, 0);
            var isSuccess = BitFields.ReadBool(bitFields, 1);

            return new MessageHead
            {
                Id = id,
                Port = port,
                IsResponse = isResponse,
                IsSuccess = isSuccess,
                DataLenght = lenght
            };
        }

        public byte[] GetBytes()
        {
            var bitFields = (byte)0;
            BitFields.SetBool(ref bitFields, 0, IsResponse);
            BitFields.SetBool(ref bitFields, 1, IsSuccess);

            var writer = new BytesWriter()
                .WriteUInt16(Id)
                .WriteByte((byte)Port)
                .WriteByte(bitFields)
                .WriteUInt32(DataLenght);

            return writer
                .GetBytes();
        }
    }
}
