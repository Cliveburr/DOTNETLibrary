using Runner.Business.Entities.Job;
using Runner.Business.Entities.Nodes.Types;

namespace Runner.Business.WatcherNotification
{
    public delegate void OnJobEventDelegate(Job job);
    public delegate void OnRunEventDelegate(Run run);

    public interface IAgentWatcherNotification
    {
        event OnJobEventDelegate? OnJobQueued;
        event OnJobEventDelegate? OnJobStop;
        event OnRunEventDelegate? OnRunUpdated;
    }
}
