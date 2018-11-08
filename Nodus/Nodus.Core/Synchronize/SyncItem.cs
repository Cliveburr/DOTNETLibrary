using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Core.Synchronize
{
    public class SyncItem
    {
        public string Token { get; set; }
        public SyncReceiveFile File { get; set; }
    }
}