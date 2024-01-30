using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Runner.Agent.Version.Scripts;
using Runner.Business.Entities.Job;
using Runner.Business.Services;
using Runner.Business.Services.NodeTypes;
using Runner.Business.WatcherNotification;
using Runner.Common.Helpers;

namespace Runner.Script.Hosting.Services
{
    public class ScriptManagerService : IDisposable
    {
        private readonly ILogger<ScriptManagerService> _logger;
        private readonly IAgentWatcherNotification _agentWatcherNotification;
        //private readonly JobService _jobService;
        //private readonly ScriptContentService _scriptContentService;
        //private readonly ScriptPackageService _scriptPackageService;
        private readonly IServiceProvider _serviceProvider;

        public ScriptManagerService(ILogger<ScriptManagerService> logger, IAgentWatcherNotification agentWatcherNotification, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _agentWatcherNotification = agentWatcherNotification;
            //_jobService = jobService;
            //_scriptContentService = scriptContentService;
            //_scriptPackageService = scriptPackageService;
            _serviceProvider = serviceProvider;

            _agentWatcherNotification.OnJobCreated += OnJobCreated;
        }

        public void Dispose()
        {
            _agentWatcherNotification.OnJobCreated -= OnJobCreated;
        }

        private void OnJobCreated(Job job)
        {
            if (job.Type == JobType.ExtractScriptPackage)
            {
                _ = ExecuteJob(job);
            }
        }

        private async Task ExecuteJob(Job job)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var jobService = scope.ServiceProvider.GetRequiredService<JobService>();
                var scriptContentService = scope.ServiceProvider.GetRequiredService<ScriptContentService>();
                var scriptPackageService = scope.ServiceProvider.GetRequiredService<ScriptPackageService>();

                try
                {
                    await jobService.SetRunning(job);

                    Assert.MustNotNull(job.ScriptContentId, "Missing ScriptContentId in job of type ExtractScriptPackage!");
                    Assert.MustNotNull(job.ScriptPackageId, "Missing ScriptPackageId in job of type ExtractScriptPackage!");
                    var scriptContent = await scriptContentService.ReadById(job.ScriptContentId.Value);
                    Assert.MustNotNull(scriptContent, $"ScriptContent in job of type ExtractScriptPackage! ScriptContentId: {job.ScriptContentId}");

                    using (var temp = TempDirectory.Create("ExtractScriptPackage"))
                    {
                        Zip.Descompat(scriptContent.FileContent, temp.Path);

                        var isolation = new ScriptIsolation(temp.Path);
                        var scriptSets = isolation.Execute();

                        await scriptPackageService.SetScripts(job.ScriptPackageId.Value, scriptContent.ScriptContentId, scriptSets);
                    }

                    await jobService.SetCompleted(job);
                }
                catch (Exception ex)
                {
                    try
                    {
                        await jobService.SetError(job, ex);
                    }
                    catch (Exception ex2)
                    {
                        _logger.LogError(ex2, null);
                    }
                }
            }
        }
    }
}
