using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Communicator.Process.FileUpload
{
    public enum FileUploadMessageType : ushort
    {
        Initiate,
        UploadPart,
        Complete,
        Cancel,
        DeleteFolder
    }
}
