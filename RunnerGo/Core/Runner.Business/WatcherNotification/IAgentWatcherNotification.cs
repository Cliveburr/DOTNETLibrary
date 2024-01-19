using Runner.Business.Entities.Job;

namespace Runner.Business.WatcherNotification
{
    public delegate void OnJobEventDelegate(Job job);
    //public delegate void OnRunEventDelegate(Run run);

    public interface IAgentWatcherNotification
    {
        event OnJobEventDelegate? OnJobCreated;
        event OnJobEventDelegate? OnStopJob;
        //event OnRunEventDelegate? OnRunUpdated;
    }
}
