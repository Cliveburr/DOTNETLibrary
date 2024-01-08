using Runner.Business.Entities.Node.Agent;
using Runner.Business.Entities.Job;
using Runner.Business.Entities.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.WatcherNotification
{
    public delegate void OnJobEventDelegate(Job job);
    public delegate void OnRunEventDelegate(Run run);

    public interface IAgentWatcherNotification
    {
        event OnJobEventDelegate? OnJobCreated;
        event OnJobEventDelegate? OnStopJob;
        event OnRunEventDelegate? OnRunUpdated;
    }
}
