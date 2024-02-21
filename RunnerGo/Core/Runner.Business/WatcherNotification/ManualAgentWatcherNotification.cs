using Runner.Business.Entities.Job;
using Runner.Business.Entities.Nodes.Types;

namespace Runner.Business.WatcherNotification
{
    public class ManualAgentWatcherNotification : IAgentWatcherNotification
    {
        public event OnJobEventDelegate? OnJobQueued;
        public event OnJobEventDelegate? OnJobStop;
        public event OnRunEventDelegate? OnRunUpdated;
        public event OnJobScheduleEventDelegate? OnJobScheduleAddOrUpdated;

        //public event OnActionEventDelegate? OnActionUpdated;

        public void InvokeJobQueued(Job job)
        {
            OnJobQueued?.Invoke(job);
        }

        public void InvokeJobStop(Job job)
        {
            OnJobStop?.Invoke(job);
        }

        public void InvokeRunUpdated(Run run)
        {
            OnRunUpdated?.Invoke(run);
        }

        //public void InvokeActionUpdated(Actions.Action action)
        //{
        //    OnActionUpdated?.Invoke(action);
        //}

        public void InvokeJobScheduleAddOrUpdated(JobSchedule jobSchedule)
        {
            OnJobScheduleAddOrUpdated?.Invoke(jobSchedule);
        }
    }
}
