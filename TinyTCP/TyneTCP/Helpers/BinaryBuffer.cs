using System;
using System.Collections.Generic;
using System.Text;

namespace TyneTCP.Helpers
{
    public class BinaryBufferOld : IDisposable
    {
        public int BufferLength { get; private set; }
        public long Length { get; private set; }

        private long _position;
        private byte[][] _buffer;
        private const int _bufferBucketSize = 10;

        private struct BucketAccess
        {
            public int Index;
            public int Position;
            public int Length;
        }

        public BinaryBufferOld(int bufferLength = 1024)
        {
            BufferLength = bufferLength;
            _buffer = new byte[0][];
        }

        public long Position
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

        private BucketAccess[] GetBucketAccesses(int count)
        {
            if (count == 0)
            {
                return new BucketAccess[0];
            }

            var index = (int)_position / BufferLength;
            var position = (int)_position % BufferLength;
            var totalBucket = ((position + count) / BufferLength) + 1;
            var buckets = new BucketAccess[totalBucket];
            var left = count;
            for (var i = 0; i < totalBucket; i++)
            {
                var length = position + left > BufferLength ?
                    BufferLength - position :
                    left;

                buckets[i].Index = index;
                buckets[i].Position = position;
                buckets[i].Length = length;

                index++;
                left -= length;
                position = 0;
            }

            return buckets;
        }

        private void SetBytes(byte[] bytes)
        {
            var buckets = GetBucketAccesses(bytes.Length);
            if (buckets.Length == 0)
            {
                return;
            }

            if (buckets[0].Index + buckets.Length > _buffer.Length)
            {
                var excedent = buckets[0].Index + buckets.Length - _buffer.Length;
                var lotes = (excedent / _bufferBucketSize) + 1;
                var totalLotes = (lotes * _bufferBucketSize) + _buffer.Length;
                var newBuffer = new byte[totalLotes][];
                for (var i = 0; i < _buffer.Length; i++)
                {
                    newBuffer[i] = _buffer[i];
                }
                _buffer = newBuffer;
            }

            var position = 0;
            foreach (var bucket in buckets)
            {
                var buffer = _buffer[bucket.Index];
                if (buffer == null)
                {
                    _buffer[bucket.Index] = buffer = new byte[BufferLength];
                }

                Buffer.BlockCopy(bytes, position, buffer, bucket.Position, bucket.Length);

                position += bucket.Length;
            }

            _position += bytes.Length;
            Length += bytes.Length;
        }

        private byte[] GetBytes(int count)
        {
            if (Position + count > Length)
            {
                throw new IndexOutOfRangeException();
            }

            var buckets = GetBucketAccesses(count);

            var bytes = new byte[count];
            var position = 0;
            foreach (var bucket in buckets)
            {
                Buffer.BlockCopy(_buffer[bucket.Index], bucket.Position, bytes, position, bucket.Length);
                position += bucket.Length;
            }

            _position += count;

            return bytes;
        }

        public void Write(byte[] bytes)
        {
            SetBytes(bytes);
        }

        public byte[] ReadBytes(int count)
        {
            return GetBytes(count);
        }

        public void WriteShort(short value)
        {
            var bytes = BitConverter.GetBytes(value);
            SetBytes(bytes);
        }

        public short ReadShort()
        {
            var bytes = ReadBytes(2);
            return BitConverter.ToInt16(bytes);
        }

        public void WriteUShort(ushort value)
        {
            var bytes = BitConverter.GetBytes(value);
            SetBytes(bytes);
        }

        public ushort ReadUShort()
        {
            var bytes = ReadBytes(2);
            return BitConverter.ToUInt16(bytes);
        }

        public void WriteInt(int value)
        {
            var bytes = BitConverter.GetBytes(value);
            SetBytes(bytes);
        }

        public int ReadInt()
        {
            var bytes = ReadBytes(4);
            return BitConverter.ToInt32(bytes);
        }

        public void WriteUInt(uint value)
        {
            var bytes = BitConverter.GetBytes(value);
            SetBytes(bytes);
        }

        public uint ReadUInt()
        {
            var bytes = ReadBytes(4);
            return BitConverter.ToUInt32(bytes);
        }

        public void WriteLong(long value)
        {
            var bytes = BitConverter.GetBytes(value);
            SetBytes(bytes);
        }

        public long ReadLong()
        {
            var bytes = ReadBytes(8);
            return BitConverter.ToInt64(bytes);
        }

        public void WriteULong(ulong value)
        {
            var bytes = BitConverter.GetBytes(value);
            SetBytes(bytes);
        }

        public ulong ReadULong()
        {
            var bytes = ReadBytes(8);
            return BitConverter.ToUInt64(bytes);
        }
    }
}