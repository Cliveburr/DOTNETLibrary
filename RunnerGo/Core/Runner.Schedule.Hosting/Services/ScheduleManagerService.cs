using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver.Core.Operations;
using MongoDB.Driver.Linq;
using Runner.Business.Entities.Job;
using Runner.Business.Helpers;
using Runner.Business.Services;
using Runner.Business.WatcherNotification;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Runner.Schedule.Hosting.Services
{
    public class ScheduleManagerService : IDisposable
    {
        private readonly ILogger<ScheduleManagerService> _logger;
        private readonly IAgentWatcherNotification _agentWatcherNotification;
        private readonly IServiceProvider _serviceProvider;
        private System.Timers.Timer _timer;

        public ScheduleManagerService(ILogger<ScheduleManagerService> logger, IAgentWatcherNotification agentWatcherNotification, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _agentWatcherNotification = agentWatcherNotification;
            _serviceProvider = serviceProvider;
            _timer = new System.Timers.Timer();
            _timer.Elapsed += CheckTickersExpired_Elpased;
            _timer.Stop();

            _agentWatcherNotification.OnJobScheduleAddOrUpdated += OnScheduleAddOrUpdated;
        }

        public void Dispose()
        {
            _agentWatcherNotification.OnJobScheduleAddOrUpdated -= OnScheduleAddOrUpdated;
        }

        private async void OnScheduleAddOrUpdated(JobSchedule? schedule)
        {
            _timer.Stop();

            using (var scope = _serviceProvider.CreateScope())
            {
                var jobScheduleService = scope.ServiceProvider.GetRequiredService<JobScheduleService>();

                if (schedule is not null)
                {
                    await jobScheduleService.DeleteTickerByJobScheduleId(schedule.JobScheduleId);

                    await CreateNextTicker(jobScheduleService, schedule, DateTime.UtcNow);
                }

                _ = CheckTickersExpired(jobScheduleService);
            }
        }

        public async Task Initialize(JobScheduleService jobScheduleService)
        {
            try
            {
                var jobMissingTickerQuery = jobScheduleService.FindActiveMissingTickers();
                foreach (var schedule in jobMissingTickerQuery)
                {
                    await CreateNextTicker(jobScheduleService, schedule, DateTime.UtcNow);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
            }

            await CheckTickersExpired(jobScheduleService);
        }

        private void CheckTickersExpired_Elpased(object? sender, ElapsedEventArgs e)
        {
            _timer.Stop();

            using (var scope = _serviceProvider.CreateScope())
            {
                var jobScheduleService = scope.ServiceProvider.GetRequiredService<JobScheduleService>();
                CheckTickersExpired(jobScheduleService)
                    .Wait(); ;
            }
        }

        private async Task CheckTickersExpired(JobScheduleService jobScheduleService)
        {
            try
            {
                var now = DateTime.UtcNow;

                var expiredTickers = jobScheduleService.FindExpiredTickers(now);
                foreach (var expired in expiredTickers)
                {
                    await jobScheduleService.DeleteTicker(expired.Ticker.JobTickerId);

                    if (!expired.Schedule.Active)
                    {
                        continue;
                    }

                    await jobScheduleService.CreateJobFromTicker(expired.Schedule);

                    await CreateNextTicker(jobScheduleService, expired.Schedule, now);
                }

                var closestTicker = await jobScheduleService.FindClosestTickerToExpire();
                if (closestTicker is not null)
                {
                    var interval = Math.Max((closestTicker.TargetUtc - now).TotalMilliseconds, 0);
                    _timer.Interval = interval;
                    _timer.Start();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
            }
        }

        private async Task CreateNextTicker(JobScheduleService jobScheduleService, JobSchedule schedule, DateTime fromUtc)
        {
            if (!schedule.Active)
            {
                return;
            }

            var targetUtc = DefineNextTickerTimer(schedule, fromUtc);
            if (targetUtc is null)
            {
                await jobScheduleService.DeactiveJobSchedule(schedule);
                return;
            }

            await jobScheduleService.CreateTicker(schedule.JobScheduleId, targetUtc.Value);
        }

        private DateTime? DefineNextTickerTimer(JobSchedule schedule, DateTime fromUtc)
        {
            switch (schedule.ScheduleType)
            {
                case JobScheduleType.Single:
                    {
                        Assert.MustNotNull(schedule.SingleDateTimeUtc, "Internal - Missing SingleDateTimeUtc in JobSchedule type Single!");
                        return schedule.SingleDateTimeUtc.Value;
                    }
                case JobScheduleType.Interval:
                    {
                        Assert.MustNotNull(schedule.IntervalSecond, "Internal - Missing IntervalSecond in JobSchedule type Interval!");
                        return fromUtc.AddSeconds(schedule.IntervalSecond.Value);
                    }
                case JobScheduleType.Daily:
                    {
                        Assert.MustNotNull(schedule.DailyTime, "Internal - Missing DailyTime in JobSchedule type Daily!");
                        Assert.MustNotNull(schedule.DaysOfWeek, "Internal - Missing DailyDayNames in JobSchedule type Daily!");

                        return ScheduleHelper.ComputNextTicker(schedule.DailyTime.Value, schedule.DaysOfWeek, fromUtc);
                    }
                default:
                    {
                        throw new RunnerException($"Internal - JobSchedule Type: \"{schedule.ScheduleType}\" is invalid! JobScheduleId: {schedule.JobScheduleId}");
                    }
            }
        }
    }
}
