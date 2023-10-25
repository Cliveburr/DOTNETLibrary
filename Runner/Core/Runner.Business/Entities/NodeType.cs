using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Entities
{
    public enum NodeType : byte
    {
        App = 0,
        Folder = 1,
        AgentPool = 2,
        Agent = 3,
        Flow = 4,
        Run = 5
    }

    public enum CreableNodeType : byte
    {
        Folder = 1,
        AgentPool = 2
    }
}
