using System;
using System.Collections.Generic;
using System.Text;

namespace TyneTCP.Helpers
{
    public class BinaryBuffer : IDisposable
    {
        public int Length { get; private set; }

        private int _position;
        private byte[] _buffer;

        public BinaryBuffer()
            : this(0)
        {
        }

        public BinaryBuffer(int bufferLength)
        {
            _buffer = new byte[bufferLength];
            Array.Clear(_buffer, 0, bufferLength);
            Length = bufferLength;
        }

        public int Position
        {
            get
            {
                return _position;
            }
            set
            {
                if ((value < 0 || value >= Length) && value != 0)
                {
                    throw new IndexOutOfRangeException();
                }
                else
                {
                    _position = value;
                }
            }
        }

        public void Dispose()
        {
            _buffer = null;
        }

        private void CheckBufferSize(int value)
        {
            if (value > _buffer.Length)
            {
                var newSize = _buffer.Length < 256 ? 256 : _buffer.Length;
                while (newSize < value)
                {
                    newSize *= 2;
                }
                //var newBuffer = new byte[newSize];
                //Buffer.BlockCopy(_buffer, 0, newBuffer, 0, _buffer.Length);
                //_buffer = newBuffer;
                Array.Resize(ref _buffer, newSize);
            }

            if (value > Length)
            {
                Array.Clear(_buffer, Length, value - Length);
                Length = value;
            }
        }

        public void Write(byte[] bytes, int offset, int count)
        {
            var lastPosition = _position + count;

            CheckBufferSize(lastPosition);

            Buffer.BlockCopy(bytes, offset, _buffer, _position, count);

            _position = lastPosition;
        }

        public byte[] ReadBytes(int count)
        {
            if (Position + count > Length)
            {
                throw new IndexOutOfRangeException();
            }

            var bytes = new byte[count];
            Buffer.BlockCopy(_buffer, _position, bytes, 0, count);

            _position += count;

            return bytes;
        }

        public void Write(byte[] bytes)
        {
            Write(bytes, 0, bytes.Length);
        }

        //public void WriteBytes(byte[] bytes)
        //{
        //    SetBytes(bytes);
        //}

        //public byte[] ReadBytes(int count)
        //{
        //    return GetBytes(count);
        //}

        //public void WriteShort(short value)
        //{
        //    var bytes = BitConverter.GetBytes(value);
        //    SetBytes(bytes);
        //}

        //public short ReadShort()
        //{
        //    var bytes = ReadBytes(2);
        //    return BitConverter.ToInt16(bytes);
        //}

        //public void WriteUShort(ushort value)
        //{
        //    var bytes = BitConverter.GetBytes(value);
        //    SetBytes(bytes);
        //}

        //public ushort ReadUShort()
        //{
        //    var bytes = ReadBytes(2);
        //    return BitConverter.ToUInt16(bytes);
        //}

        //public void WriteInt(int value)
        //{
        //    var bytes = BitConverter.GetBytes(value);
        //    SetBytes(bytes);
        //}

        //public int ReadInt()
        //{
        //    var bytes = ReadBytes(4);
        //    return BitConverter.ToInt32(bytes);
        //}

        //public void WriteUInt(uint value)
        //{
        //    var bytes = BitConverter.GetBytes(value);
        //    SetBytes(bytes);
        //}

        //public uint ReadUInt()
        //{
        //    var bytes = ReadBytes(4);
        //    return BitConverter.ToUInt32(bytes);
        //}

        //public void WriteLong(long value)
        //{
        //    var bytes = BitConverter.GetBytes(value);
        //    SetBytes(bytes);
        //}

        //public long ReadLong()
        //{
        //    var bytes = ReadBytes(8);
        //    return BitConverter.ToInt64(bytes);
        //}

        //public void WriteULong(ulong value)
        //{
        //    var bytes = BitConverter.GetBytes(value);
        //    SetBytes(bytes);
        //}

        //public ulong ReadULong()
        //{
        //    var bytes = ReadBytes(8);
        //    return BitConverter.ToUInt64(bytes);
        //}
    }
}