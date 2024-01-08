using Runner.Business.Entities.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Entities.FileStorage
{
    public class FileStorage : DocumentBase
    {
        public required string FileName { get; set; }
        public required byte[] Content { get; set; }
    }
}
