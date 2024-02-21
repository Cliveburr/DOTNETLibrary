using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Runner.Business.Entities.Job;
using Runner.Business.Security;
using Runner.Business.Services;
using Runner.Business.WatcherNotification;
using Runner.Common.Helpers;

namespace Runner.Business.Jobs
{
    public class JobMediator : IDisposable
    {
        private readonly ILogger<JobMediator> _logger;
        private readonly IAgentWatcherNotification _agentWatcherNotification;
        private readonly IServiceProvider _serviceProvider;
        private readonly List<JobHandler> _jobHandlers;
        private readonly OneExecutionAtTime _checkJobsWaiting;

        private class JobHandler
        {
            public required Type Type { get; set; }
            public required JobType JobType { get; set; }
        }

        public JobMediator(ILogger<JobMediator> logger, IAgentWatcherNotification agentWatcherNotification, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _agentWatcherNotification = agentWatcherNotification;
            _serviceProvider = serviceProvider;
            _jobHandlers = new List<JobHandler>();
            _checkJobsWaiting = new OneExecutionAtTime(CheckJobsWaiting);

            _agentWatcherNotification.OnJobQueued += OnJobQueued;
        }

        public void Dispose()
        {
            _agentWatcherNotification.OnJobQueued -= OnJobQueued;
        }

        public JobMediator AddJobHandler<T>(JobType jobType) where T : IJobHandler
        {
            var alreadyHas = _jobHandlers
                .Where(t => t.JobType == jobType)
                .Any();
            Assert.MustFalse(alreadyHas, "Internal - Only one handler for JobType is allow!");

            _jobHandlers.Add(new JobHandler
            {
                Type = typeof(T),
                JobType = jobType
            });
            return this;
        }

        private void OnJobQueued(Job job)
        {
            _ = ExecuteJob(job);
        }

        public void FlagToCheckJobsWaiting()
        {
            _checkJobsWaiting.Execute();
        }

        private async Task CheckJobsWaiting()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var jobService = scope.ServiceProvider.GetRequiredService<JobService>();

                var job = await jobService.GetWaiting();
                while (job is not null)
                {
                    _ = ExecuteJob(job);
                    job = await jobService.GetWaiting();
                }
            }
        }

        private async Task ExecuteJob(Job job)
        {
            JobService? jobService = null;

            using (var scope = _serviceProvider.CreateScope())
            {
                try
                {
                    jobService = scope.ServiceProvider.GetRequiredService<JobService>();
                    var authenticationService = scope.ServiceProvider.GetRequiredService<AuthenticationService>();
                    authenticationService.LoginForInternalServices();

                    var jobHandler = _jobHandlers
                        .FirstOrDefault(jh => jh.JobType == job.Type);
                    Assert.MustNotNull(jobHandler, $"JobHandler not found for JobType: {job.Type}, JobId: {job.JobId}");

                    var handler = scope.ServiceProvider.GetService(jobHandler.Type) as IJobHandler;
                    Assert.MustNotNull(handler, $"JobHandler service not found or invalid for Type: {jobHandler.Type}, JobId: {job.JobId}");

                    await jobService.SetRunning(job.JobId);

                    var resolved = await handler.Execute(job);
                    if (resolved)
                    {
                        await jobService.SetCompleted(job.JobId);
                    }
                    else
                    {
                        await jobService.SetWaiting(job.JobId);
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        Assert.MustNotNull(jobService, "Internal - JobService not found!");
                        await jobService.SetError(job.JobId, ex);
                    }
                    catch (Exception ex2)
                    {
                        _logger.LogError(new AggregateException(ex, ex2), null);
                    }
                }
            }
        }
    }
}
