using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Entities.Tasks
{
    public enum TaskScheduleStatus : byte
    {
        Waiting = 0,
        Running = 1,
        Completed = 2,
        Error = 3
    }
}
