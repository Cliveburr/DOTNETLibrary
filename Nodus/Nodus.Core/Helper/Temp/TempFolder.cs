using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Core.Helper.Temp
{
    public class TempFolder : IDisposable
    {
        public string Folder { get; private set; }

        public TempFolder(string folder)
        {
            Folder = folder;
        }

        public void Dispose()
        {
            IO.DeleteDirectory(Folder, "*", true);
        }
    }
}