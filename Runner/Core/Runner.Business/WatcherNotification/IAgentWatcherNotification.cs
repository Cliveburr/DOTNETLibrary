using Runner.Business.Entities;
using Runner.Business.Entities.Agent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.WatcherNotification
{
    public delegate void OnJobCreatedDelegate(Job job);

    public interface IAgentWatcherNotification
    {
        event OnJobCreatedDelegate? OnJobCreated;
        void InvokeJobCreated(Job job);
    }
}
