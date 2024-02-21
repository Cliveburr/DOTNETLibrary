using Runner.Business.Entities.Job;
using Runner.Business.Entities.Nodes.Types;

namespace Runner.Business.WatcherNotification
{
    public delegate void OnJobEventDelegate(Job job);
    public delegate void OnRunEventDelegate(Run run);
    public delegate void OnActionEventDelegate(Actions.Action action);
    public delegate void OnJobScheduleEventDelegate(JobSchedule schedule);

    public interface IAgentWatcherNotification
    {
        event OnJobEventDelegate? OnJobQueued;
        event OnRunEventDelegate? OnRunUpdated;
        //event OnActionEventDelegate? OnActionUpdated;
        event OnJobScheduleEventDelegate? OnJobScheduleAddOrUpdated;
    }
}
