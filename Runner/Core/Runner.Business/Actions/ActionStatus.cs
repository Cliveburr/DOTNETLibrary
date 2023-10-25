using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Actions
{
    public enum ActionStatus : byte
    {
        Waiting = 0,
        Running = 1,
        Stopped = 2,
        Error = 3,
        Completed = 4
    }
}
