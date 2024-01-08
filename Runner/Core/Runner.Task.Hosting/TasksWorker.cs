using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Runner.Business.Entities.Tasks;
using Runner.Business.Services;
using Runner.Task.Hosting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Task.Hosting
{
    public class TasksWorker : Microsoft.Extensions.Hosting.BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private List<TaskStorage>? _tasks;

        public TasksWorker(ILogger<TasksWorker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        private Dictionary<TaskType, Type> InitializeTasks()
        {
            return new Dictionary<TaskType, Type>
            {
                { TaskType.ProcessScript, typeof(Tasks.ProcessScriptTask) },
            };
        }

        protected override async System.Threading.Tasks.Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateAsyncScope())
            {
                var taskService = scope.ServiceProvider.GetRequiredService<TaskService>();

                var tasksToRun = InitializeTasks();
                var allTaskTypes = tasksToRun
                    .Select(t => t.Key)
                    .ToList();

                var allTasks = await taskService.GetTasksEnabled(allTaskTypes);
                _tasks = new List<TaskStorage>();

                foreach (var taskToRun in tasksToRun)
                {
                    var task = allTasks
                        .FirstOrDefault(t => t.Type == taskToRun.Key);
                    if (task is not null)
                    {
                        _tasks.Add(new TaskStorage
                        {
                            Id = task.Id,
                            Type = taskToRun.Key,
                            TaskType = taskToRun.Value
                        });
                    }
                }

                //var allSchedule = taskService.GetSchedules();
            }

            // pegar todas tasks schedule e agendar timer para executar

            // se vincular ao watcher

            // rodar o timer
            await CheckSchedules();
        }

        private async System.Threading.Tasks.Task CheckSchedules()
        {
            var thisRunDatetime = DateTime.UtcNow;

            using (var scope = _serviceProvider.CreateAsyncScope())
            {
                var taskService = scope.ServiceProvider.GetRequiredService<TaskService>();

                var tasksToRun = InitializeTasks();
                var allTaskTypes = tasksToRun
                    .Select(t => t.Key)
                    .ToList();

                var allTasks = await taskService.GetTasksEnabled(allTaskTypes);
                if (!allTasks.Any())
                {
                    return;
                }
                //_tasks = new List<TaskStorage>();

                //foreach (var taskToRun in tasksToRun)
                //{
                //    var task = allTasks
                //        .FirstOrDefault(t => t.Type == taskToRun.Key);
                //    if (task is not null)
                //    {
                //        _tasks.Add(new TaskStorage
                //        {
                //            Id = task.Id,
                //            Type = taskToRun.Key,
                //            TaskType = taskToRun.Value
                //        });
                //    }
                //}
                var allTasksId = allTasks
                    .Select(t => t.Id)
                    .ToList();

                var allSchedule = await taskService.GetSchedulesExpired(thisRunDatetime, allTasksId);
                if (!allSchedule.Any())
                {
                    return;
                }

                foreach (var schedule in allSchedule)
                {
                    var timeSchedule = schedule.ExecutionUTC - thisRunDatetime;
                    if (timeSchedule <= TimeSpan.Zero)
                    {

                    }
                    else
                    {

                    }
                }
            }
        }
    }
}
