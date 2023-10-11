using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Runner.Communicator.Helpers;

namespace Runner.Communicator.Process.FileUpload.Model
{
    public class CancelUploadRequest
    {
        public string? FileId { get; set; }

        public static CancelUploadRequest Parse(BytesReader reader)
        {
            var fileId = reader.ReadString();

            return new CancelUploadRequest
            {
                FileId = fileId
            };
        }

        public byte[] GetBytes()
        {
            return new BytesWriter()
                .WriteUInt16Enum(FileUploadMessageType.Cancel)
                .WriteString(FileId)
                .GetBytes();
        }
    }
}
