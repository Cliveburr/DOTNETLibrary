using Runner.Business.Entities.Agent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.WatcherNotification
{
    public delegate void OnRunScriptDelegate(Agent agent);

    public interface IAgentWatcherNotification
    {
        event OnRunScriptDelegate? OnRunScript;
        void InvokeRunScript(Agent agent);
    }
}
