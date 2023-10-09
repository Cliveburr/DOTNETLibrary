using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Communication.Helpers
{
    public class BytesWriter : IDisposable
    {
        private MemoryStream _mem;
        private BinaryWriter _writer;

        public BytesWriter()
        {
            _mem = new MemoryStream();
            _writer = new BinaryWriter(_mem);
        }

        public void Dispose()
        {
            _mem.Dispose();
            _writer.Dispose();
        }

        public byte[] GetBytes()
        {
            return _mem.ToArray();
        }

        public BytesWriter WriteByte(byte value)
        {
            _writer.Write(value);
            return this;
        }

        public BytesWriter WriteBytes(byte[] value)
        {
            _writer.Write(value);
            return this;
        }

        public BytesWriter WriteUInt32Bytes(byte[]? value)
        {
            if (value == null)
            {
                _writer.Write((uint)0);
            }
            else
            {
                _writer.Write((uint)(value.Length + 1));
                _writer.Write(value);
            }
            return this;
        }

        public BytesWriter WriteUInt32DoubleBytes(byte[]?[]? value)
        {
            if (value == null)
            {
                _writer.Write((uint)0);
            }
            else
            {
                _writer.Write((uint)(value.Length + 1));
                for (var i = 0; i < value.Length; i++)
                {
                    WriteUInt32Bytes(value[i]);
                }
            }
            return this;
        }

        public BytesWriter WriteBool(bool value)
        {
            _writer.Write(value ? (byte)1 : (byte)0);
            return this;
        }

        public BytesWriter WriteInt16(short value)
        {
            _writer.Write(value);
            return this;
        }

        public BytesWriter WriteUInt16(ushort value)
        {
            _writer.Write(value);
            return this;
        }

        public BytesWriter WriteUInt16Enum<T>(T value) where T: Enum
        {
            var enumValue = Unsafe.As<T, ushort>(ref value);
            _writer.Write(enumValue);
            return this;
        }

        public BytesWriter WriteInt32(int value)
        {
            _writer.Write(value);
            return this;
        }

        public BytesWriter WriteUInt32(uint value)
        {
            _writer.Write(value);
            return this;
        }

        public BytesWriter WriteInt64(long value)
        {
            _writer.Write(value);
            return this;
        }

        public BytesWriter WriteUInt64(ulong value)
        {
            _writer.Write(value);
            return this;
        }

        public BytesWriter WriteString(string? text)
        {
            if (text == null)
            {
                _writer.Write((ushort)0);
            }
            else
            {
                _writer.Write((ushort)(text.Length + 1));
                _writer.Write(Encoding.UTF8.GetBytes(text));
            }
            return this;
        }
    }
}
