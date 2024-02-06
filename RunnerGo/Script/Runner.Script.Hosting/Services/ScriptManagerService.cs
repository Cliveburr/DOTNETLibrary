using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Runner.Agent.Version.Scripts;
using Runner.Business.Entities.Job;
using Runner.Business.Services;
using Runner.Business.Services.NodeTypes;
using Runner.Business.WatcherNotification;
using Runner.Common.Helpers;
using System.Text;

namespace Runner.Script.Hosting.Services
{
    public class ScriptManagerService : IDisposable
    {
        private readonly ILogger<ScriptManagerService> _logger;
        private readonly IAgentWatcherNotification _agentWatcherNotification;
        private readonly IServiceProvider _serviceProvider;

        public ScriptManagerService(ILogger<ScriptManagerService> logger, IAgentWatcherNotification agentWatcherNotification, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _agentWatcherNotification = agentWatcherNotification;
            _serviceProvider = serviceProvider;

            _agentWatcherNotification.OnJobQueued += OnJobQueued;
        }

        public void Dispose()
        {
            _agentWatcherNotification.OnJobQueued -= OnJobQueued;
        }

        private void OnJobQueued(Job job)
        {
            if (job.Type == JobType.ExtractScriptPackage)
            {
                _ = ExecuteJob(job);
            }
        }

        public async Task CheckJobsForExtractScript()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var jobService = scope.ServiceProvider.GetRequiredService<JobService>();

                var job = await jobService.GetWaitingAndQueueExtractScript();
                while (job is not null)
                {
                    await ExecuteJob(job);
                    job = await jobService.GetWaitingAndQueueExtractScript();
                }
            }
        }

        private async Task ExecuteJob(Job job)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var jobService = scope.ServiceProvider.GetRequiredService<JobService>();
                var scriptContentService = scope.ServiceProvider.GetRequiredService<ScriptContentService>();
                var scriptPackageService = scope.ServiceProvider.GetRequiredService<ScriptPackageService>();
                var warnings = new StringBuilder();
                var cleanScriptContent = true;

                try
                {
                    await jobService.SetRunning(job.JobId);

                    Assert.MustNotNull(job.ScriptContentId, "Missing ScriptContentId in job of type ExtractScriptPackage!");
                    Assert.MustNotNull(job.ScriptPackageId, "Missing ScriptPackageId in job of type ExtractScriptPackage!");
                    var scriptContent = await scriptContentService.ReadById(job.ScriptContentId.Value);
                    Assert.MustNotNull(scriptContent, $"ScriptContent in job of type ExtractScriptPackage! ScriptContentId: {job.ScriptContentId}");

                    using (var temp = TempDirectory.Create("ExtractScriptPackage"))
                    {
                        Zip.Descompat(scriptContent.FileContent, temp.Path);

                        var isolation = new ScriptIsolation(temp.Path);
                        var scriptSets = isolation.Execute(warnings);

                        cleanScriptContent = false;
                        await scriptPackageService.SetScripts(job.ScriptPackageId.Value, scriptContent.ScriptContentId, scriptSets, warnings);
                    }

                    await jobService.SetCompleted(job.JobId);
                }
                catch (Exception ex)
                {
                    warnings.AppendLine("Error: " + ex.Message);

                    if (cleanScriptContent && job.ScriptContentId is not null)
                    {
                        try
                        {
                            await scriptContentService.Delete(job.ScriptContentId.Value);
                        }
                        catch (Exception ex2)
                        {
                            warnings.AppendLine("Error: " + ex2.Message);
                            _logger.LogError(ex2, null);
                        }
                    }

                    try
                    {
                        await jobService.SetError(job.JobId, ex);
                    }
                    catch (Exception ex2)
                    {
                        warnings.AppendLine("Error: " + ex2.Message);
                        _logger.LogError(ex2, null);
                    }
                }

                if (warnings.Length > 0 && job.ScriptPackageId is not null)
                {
                    try
                    {
                        await scriptPackageService.UpdateWarningAndClearJob(job.ScriptPackageId.Value, warnings);
                    }
                    catch (Exception ex)
                    {
                        warnings.AppendLine("Error: " + ex.Message);
                        _logger.LogError(ex, null);
                    }
                }
            }
        }
    }
}
