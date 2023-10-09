using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Communication.Helpers
{
    public class BytesReader
    {
        private MemoryStream _mem;
        private BinaryReader _reader;

        public BytesReader(byte[] buffer)
        {
            _mem = new MemoryStream(buffer);
            _reader = new BinaryReader(_mem);
        }

        public byte ReadByte()
        {
            return _reader.ReadByte();
        }

        public byte[] ReadBytes(uint argsLen)
        {
            return _reader.ReadBytes((int)argsLen);
        }

        public byte[]? ReadUInt32Bytes()
        {
            var len = _reader.ReadUInt32();
            if (len == 0)
            {
                return null;
            }
            else
            {
                return _reader.ReadBytes((int)len - 1);
            }
        }

        public byte[]?[]? ReadUInt32DoubleBytes()
        {
            var len0 = _reader.ReadUInt32();
            if (len0 == 0)
            {
                return null;
            }
            else
            {
                byte[]?[]? ret = new byte[len0 - 1][];
                for (var i = 0; i < ret.Length; i ++)
                {
                    ret[i] = ReadUInt32Bytes();
                }
                return ret;
            }
        }

        public bool ReadBool()
        {
            return _reader.ReadByte() == 1;
        }

        public short ReadInt16()
        {
            return _reader.ReadInt16();
        }

        public ushort ReadUInt16()
        {
            return _reader.ReadUInt16();
        }

        public T ReadUInt16Enum<T>() where T : Enum
        {
            var value = _reader.ReadUInt16();
            return Unsafe.As<ushort, T>(ref value);
        }

        public int ReadInt32()
        {
            return _reader.ReadInt32();
        }

        public uint ReadUInt32()
        {
            return _reader.ReadUInt32();
        }

        public long ReadInt64()
        {
            return _reader.ReadInt64();
        }

        public ulong ReadUInt64()
        {
            return _reader.ReadUInt64();
        }

        public string? ReadString()
        {
            var len = (int)_reader.ReadUInt16();
            if (len == 0)
            {
                return null;
            }
            else
            {
                return Encoding.UTF8.GetString(_reader.ReadBytes(len - 1));
            }
        }
    }
}
