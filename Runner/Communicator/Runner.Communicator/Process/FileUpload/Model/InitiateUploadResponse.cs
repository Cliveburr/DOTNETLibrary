using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Runner.Communicator.Helpers;

namespace Runner.Communicator.Process.FileUpload.Model
{
    public class InitiateUploadResponse
    {
        public string? FileId { get; set; }

        public static InitiateUploadResponse Parse(byte[] data)
        {
            var reader = new BytesReader(data);
            var fileId = reader.ReadString();

            return new InitiateUploadResponse
            {
                FileId = fileId
            };
        }

        public byte[] GetBytes()
        {
            return new BytesWriter()
                .WriteString(FileId)
                .GetBytes();
        }
    }
}
