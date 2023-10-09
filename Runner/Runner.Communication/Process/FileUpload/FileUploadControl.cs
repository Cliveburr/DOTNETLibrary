using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Communication.Process.FileUpload
{
    public class FileUploadControl
    {
        public required string FileId { get; set; }
        public required string DestineFilePath { get; set; }
        public required string DestineFilePathUpload { get; set; }
        public DateTime LastIteration { get; set; }
        public required FileStream Stream { get; set; }
    }
}
