using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Entities.Job
{
    public enum JobStatus
    {
        Waiting = 0,
        Running = 1,
        Error = 2,
        Completed = 3
    }
}
