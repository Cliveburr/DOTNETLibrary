using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Entities.Node.Agent
{
    public enum AgentStatus : byte
    {
        Offline = 0,
        Idle = 1,
        Running = 2
    }
}
