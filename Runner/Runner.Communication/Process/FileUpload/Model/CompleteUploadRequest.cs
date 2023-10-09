using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Runner.Communication.Helpers;

namespace Runner.Communication.Process.FileUpload.Model
{
    public class CompleteUploadRequest
    {
        public string? FileId { get; set; }
        public string? Checksum { get; set; }

        public static CompleteUploadRequest Parse(BytesReader reader)
        {
            var fileId = reader.ReadString();
            var checksum = reader.ReadString();

            return new CompleteUploadRequest
            {
                FileId = fileId,
                Checksum = checksum
            };
        }

        public byte[] GetBytes()
        {
            return new BytesWriter()
                .WriteUInt16Enum(FileUploadMessageType.Complete)
                .WriteString(FileId)
                .WriteString(Checksum)
                .GetBytes();
        }
    }
}
