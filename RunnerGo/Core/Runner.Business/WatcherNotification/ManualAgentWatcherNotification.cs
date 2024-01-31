using Runner.Business.Entities.Job;
using Runner.Business.Entities.Nodes.Types;

namespace Runner.Business.WatcherNotification
{
    public class ManualAgentWatcherNotification : IAgentWatcherNotification
    {
        public event OnJobEventDelegate? OnJobCreated;
        public event OnJobEventDelegate? OnStopJob;
        public event OnRunEventDelegate? OnRunUpdated;

        public void InvokeJobCreated(Job job)
        {
            OnJobCreated?.Invoke(job);
        }

        public void InvokeRunUpdated(Run run)
        {
            OnRunUpdated?.Invoke(run);
        }

        public void InvokeStopJob(Job job)
        {
            OnStopJob?.Invoke(job);
        }
    }
}
