using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Runner.Communication.Helpers;

namespace Runner.Communication.Process.FileUpload.Model
{
    public class UploadPartRequest
    {
        public string? FileId { get; set; }
        public long FilePosition { get; set; }
        public long PartSize { get; set; }
        public string? Checksum { get; set; }
        public byte[]? Data { get; set; }

        public static UploadPartRequest Parse(BytesReader reader)
        {
            var fileId = reader.ReadString();
            var filePosition = reader.ReadInt64();
            var partSize = reader.ReadInt64();
            var checksum = reader.ReadString();
            var data = reader.ReadUInt32Bytes();

            return new UploadPartRequest
            {
                FileId = fileId,
                FilePosition = filePosition,
                PartSize = partSize,
                Checksum = checksum,
                Data = data
            };
        }

        public byte[] GetBytes()
        {
            return new BytesWriter()
                .WriteUInt16Enum(FileUploadMessageType.UploadPart)
                .WriteString(FileId)
                .WriteInt64(FilePosition)
                .WriteInt64(PartSize)
                .WriteString(Checksum)
                .WriteUInt32Bytes(Data)
                .GetBytes();
        }
    }
}
