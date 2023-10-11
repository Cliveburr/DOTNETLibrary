using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Runner.Communicator.Helpers;

namespace Runner.Communicator.Process.FileUpload.Model
{
    public class DeleteFolderRequest
    {
        public string? Folder { get; set; }

        public static DeleteFolderRequest Parse(BytesReader reader)
        {
            var folder = reader.ReadString();

            return new DeleteFolderRequest
            {
                Folder = folder
            };
        }

        public byte[] GetBytes()
        {
            return new BytesWriter()
                .WriteUInt16Enum(FileUploadMessageType.DeleteFolder)
                .WriteString(Folder)
                .GetBytes();
        }
    }
}
