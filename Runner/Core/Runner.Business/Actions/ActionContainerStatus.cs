using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Actions
{
    public enum ActionContainerStatus : byte
    {
        StandBy = 0,
        Ready = 1,
        Done = 2
    }
}
