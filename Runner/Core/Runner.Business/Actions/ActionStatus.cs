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
        Ready = 1,
        Running = 2,
        Stopped = 3,
        Error = 4,
        Completed = 5
    }
}
