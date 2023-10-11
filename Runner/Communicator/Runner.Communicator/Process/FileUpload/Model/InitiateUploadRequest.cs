using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Runner.Communicator.Helpers;

namespace Runner.Communicator.Process.FileUpload.Model
{
    public class InitiateUploadRequest
    {
        public string? DestineFilePath { get; set; }
        public bool Overwrite { get; set; }
        public long FileLength { get; set; }

        public static InitiateUploadRequest Parse(BytesReader reader)
        {
            var destineFilePath = reader.ReadString();
            var overwrite = reader.ReadBool();
            var fileLength = reader.ReadInt64();

            return new InitiateUploadRequest
            {
                DestineFilePath = destineFilePath,
                Overwrite = overwrite,
                FileLength = fileLength
            };
        }

        public byte[] GetBytes()
        {
            return new BytesWriter()
                .WriteUInt16Enum(FileUploadMessageType.Initiate)
                .WriteString(DestineFilePath)
                .WriteBool(Overwrite)
                .WriteInt64(FileLength)
                .GetBytes();
        }
    }
}
