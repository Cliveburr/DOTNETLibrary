using Runner.Communicator.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Communicator.Process.Services2
{
    public class InvokeResponse
    {
        public bool IsSuccess { get; set; }
        public byte[]? Result { get; set; }

        public static InvokeResponse Parse(byte[] buffer)
        {
            var reader = new BytesReader(buffer);
            var isSuccess = reader.ReadBool();
            var result = reader.ReadUInt32Bytes();

            return new InvokeResponse
            {
                IsSuccess = isSuccess,
                Result = result
            };
        }

        public byte[] GetBytes()
        {
            return new BytesWriter()
                .WriteBool(IsSuccess)
                .WriteUInt32Bytes(Result)
                .GetBytes();
        }
    }
}
