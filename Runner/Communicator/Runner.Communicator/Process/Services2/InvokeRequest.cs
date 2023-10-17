using Runner.Communicator.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Communicator.Process.Services2
{
    public class InvokeRequest
    {
        public string? AssemblyQualifiedName { get; set; }
        public string? Method { get; set; }
        public byte[]?[]? Args { get; set; }

        public static InvokeRequest Parse(byte[] buffer)
        {
            var reader = new BytesReader(buffer);
            var assemblyQualifiedName = reader.ReadString();
            var method = reader.ReadString();
            var args = reader.ReadUInt32DoubleBytes();

            return new InvokeRequest
            {
                AssemblyQualifiedName = assemblyQualifiedName,
                Method = method,
                Args = args
            };
        }

        public byte[] GetBytes()
        {
            return new BytesWriter()
                .WriteString(AssemblyQualifiedName)
                .WriteString(Method)
                .WriteUInt32DoubleBytes(Args)
                .GetBytes();
        }
    }
}
