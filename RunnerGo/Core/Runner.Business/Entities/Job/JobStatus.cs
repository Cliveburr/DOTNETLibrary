
namespace Runner.Business.Entities.Job
{
    public enum JobStatus : byte
    {
        Waiting = 0,
        Running = 1,
        Error = 2,
        Completed = 3
    }
}
