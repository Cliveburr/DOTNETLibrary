
namespace Runner.Business.Entities.Job
{
    public enum JobStatus : byte
    {
        Queued = 0,
        Waiting = 1,
        Running = 2,
        Error = 3,
        Completed = 4
    }
}
