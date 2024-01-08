using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Script.Interface.Data
{
    public enum DataWriterCommand : byte
    {
        Set = 0,
        Delete = 1
    }
}
