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
        private readonly JobScheduleService _jobScheduleService;
        private System.Timers.Timer _timer;

        public ScheduleManagerService(ILogger<ScheduleManagerService> logger, IAgentWatcherNotification agentWatcherNotification, IServiceProvider serviceProvider, JobScheduleService jobScheduleService)
        {
            _logger = logger;
            _agentWatcherNotification = agentWatcherNotification;
            _serviceProvider = serviceProvider;
            _jobScheduleService = jobScheduleService;
            _timer = new System.Timers.Timer();
            _timer.Elapsed += CheckTickersExpired_Elpased;
            _timer.Stop();

            _agentWatcherNotification.OnJobScheduleAddOrUpdated += OnScheduleAddOrUpdated;
        }

        public void Dispose()
        {
            _agentWatcherNotification.OnJobScheduleAddOrUpdated -= OnScheduleAddOrUpdated;
        }

        private async void OnScheduleAddOrUpdated(JobSchedule schedule)
        {
            await _jobScheduleService.DeleteTickerByJobScheduleId(schedule.JobScheduleId);

            await CreateNextTicker(schedule, DateTime.UtcNow);

            _timer.Stop();
            _ = CheckTickersExpired();
        }

        public async Task Initialize()
        {
            try
            {
                var jobMissingTickerQuery = _jobScheduleService.FindActiveMissingTickers();
                foreach (var schedule in jobMissingTickerQuery)
                {
                    await CreateNextTicker(schedule, DateTime.UtcNow);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
            }

            await CheckTickersExpired();
        }

        private void CheckTickersExpired_Elpased(object? sender, ElapsedEventArgs e)
        {
            _timer.Stop();
            _ = CheckTickersExpired();
        }

        private async Task CheckTickersExpired()
        {
            try
            {
                var now = DateTime.UtcNow;

                var expiredTickers = _jobScheduleService.FindExpiredTickers(now);
                foreach (var expired in expiredTickers)
                {
                    await _jobScheduleService.DeleteTicker(expired.Ticker.JobTickerId);

                    if (expired.Schedule is null || !expired.Schedule.Active)
                    {
                        continue;
                    }

                    await _jobScheduleService.CreateJob(expired.Schedule);

                    await CreateNextTicker(expired.Schedule, now);
                }

                var closestTicker = await _jobScheduleService.FindClosestTickerToExpire();
                if (closestTicker is not null)
                {
                    var interval = Math.Min((closestTicker.TargetUtc - now).TotalMilliseconds, 0);
                    _timer.Interval = interval;
                    _timer.Start();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
            }
        }

        private async Task CreateNextTicker(JobSchedule schedule, DateTime from)
        {
            if (!schedule.Active)
            {
                return;
            }

            var targetUtc = DefineNextTickerTimer(schedule, from);
            if (targetUtc is null)
            {
                return;
            }

            await _jobScheduleService.CreateTicker(schedule.JobScheduleId, targetUtc.Value);
        }

        private DateTime? DefineNextTickerTimer(JobSchedule schedule, DateTime from)
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
                        return from.AddSeconds(schedule.IntervalSecond.Value);
                    }
                case JobScheduleType.Daily:
                    {
                        Assert.MustNotNull(schedule.DailyTime, "Internal - Missing DailyTime in JobSchedule type Daily!");
                        Assert.MustNotNull(schedule.DailyDayNames, "Internal - Missing DailyDayNames in JobSchedule type Daily!");

                        return ScheduleHelper.ComputNextTicker(schedule.DailyTime.Value, schedule.DailyDayNames, from);
                    }
                default:
                    {
                        throw new RunnerException($"Internal - JobSchedule Type: \"{schedule.ScheduleType}\" is invalid! JobScheduleId: {schedule.JobScheduleId}");
                    }
            }
        }
    }
}
