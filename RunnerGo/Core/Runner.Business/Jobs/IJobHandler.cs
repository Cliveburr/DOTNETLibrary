using Runner.Business.Entities.Job;

namespace Runner.Business.Jobs
{
    public interface IJobHandler
    {
        Task<bool> Execute(Job job);
    }
}
