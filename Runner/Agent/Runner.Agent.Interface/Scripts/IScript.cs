using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Agent.Interface.Scripts
{
    public interface IScript
    {
        Task Run(ScriptRunContext context);
    }
}
