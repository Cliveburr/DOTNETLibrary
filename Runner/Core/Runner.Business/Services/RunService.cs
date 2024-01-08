using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Runner.Business.Actions;
using Runner.Business.Authentication;
using Runner.Business.DataAccess;
using Runner.Business.Entities.Node.Agent;
using Runner.Business.Entities.Node;
using Runner.Business.WatcherNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Runner.Business.Services
{
    public class RunService : ServiceBase
    {
        private readonly UserLogged _userLogged;
        private readonly JobService _jobService;
        private ManualAgentWatcherNotification _manualAgentWatcherNotification;

        public RunService(Database database, JobService jobService, UserLogged userLogged, IAgentWatcherNotification agentWatcherNotification)
            : base(database)
        {
            _jobService = jobService;
            _userLogged = userLogged;
            _manualAgentWatcherNotification = (ManualAgentWatcherNotification)agentWatcherNotification;
        }

        public async Task CreateRun(Flow flow, bool setToRun)
        {
            Assert.MustNotNull(_userLogged.User, "Need to be logged to create app!");

            // checar se ter permissão de Create no parent

            var run = ActionControl.Build(flow);

            run.Type = NodeType.Run;
            run.Parent = flow.Id;
            run.Status = RunStatus.Waiting;
            run.Created = DateTime.Now;
            if (setToRun)
            {
                run.Log.Add(new RunLog
                {
                    Created = DateTime.Now,
                    Text = "New run was created"
                });
            }
            else
            {
                run.Log.Add(new RunLog
                {
                    Created = DateTime.Now,
                    Text = "New run was created in waiting"
                });
            }

            // start transaction
            await Node.CreateAsync(run);

            if (setToRun)
            {
                var control = ActionControl.From(run);

                var effects = control.Run(run.RootActionId);

                await ProcessEffects(run, effects);
            }
        }

        private async Task ProcessEffects(Run run, List<CommandEffect> effects)
        {
            foreach (var effect in effects)
            {
                switch (effect.Type)
                {
                    case ComandEffectType.ActionUpdateStatus:
                        {
                            await ExecuteActionUpdateStatus(run, effect.Action);
                            break;
                        }
                    case ComandEffectType.ActionUpdateToRun:
                        {
                            await ExecuteActionUpdateToRun(run, effect.Action);
                            break;
                        }
                    case ComandEffectType.ActionUpdateWithCursor:
                        {
                            await ExecuteActionUpdateWithCursor(run, effect.Action);
                            break;
                        }
                    case ComandEffectType.ActionUpdateBreakPoint:
                        {
                            await ExecuteActionUpdateBreakPoint(run, effect.Action);
                            break;
                        }
                    case ComandEffectType.ActionUpdateToStop:
                        {
                            await ExecuteActionUpdateToStop(run, effect.Action);
                            break;
                        }
                    default:
                        throw new Exception("Invalid CommandEffect Type: " + effect.Type);
                }
            }

            await CheckRunState(run.Id);
            _manualAgentWatcherNotification.InvokeRunUpdated(run);
        }

        private async Task ExecuteActionUpdateToRun(Run run, Actions.Action action)
        {
            await ExecuteActionUpdateStatus(run, action);

            await _jobService.CreateJob(run, action);
        }

        private async Task ExecuteActionUpdateStatus(Run run, Actions.Action action)
        {
            //var filter = Builders<Run>.Filter
            //    .Where(r => r.Id == run.Id && r.Actions.Any(a => a.ActionId == action.ActionId));

            var update = Builders<Run>.Update
                .Set(r => r.Actions.FirstMatchingElement().Status, action.Status);

            await Node.UpdateAsync(r => r.Id == run.Id && r.Actions.Any(a => a.ActionId == action.ActionId), update);
        }

        private async Task ExecuteActionUpdateWithCursor(Run run, Actions.Action action)
        {
            var update = Builders<Run>.Update
                .Set(r => r.Actions.FirstMatchingElement().WithCursor, action.WithCursor);

            await Node.UpdateAsync(r => r.Id == run.Id && r.Actions.Any(a => a.ActionId == action.ActionId), update);
        }

        private async Task ExecuteActionUpdateBreakPoint(Run run, Actions.Action action)
        {
            var update = Builders<Run>.Update
                .Set(r => r.Actions.FirstMatchingElement().BreakPoint, action.BreakPoint);

            await Node.UpdateAsync(r => r.Id == run.Id && r.Actions.Any(a => a.ActionId == action.ActionId), update);
        }

        private async Task ExecuteActionUpdateToStop(Run run, Actions.Action action)
        {
            await ExecuteActionUpdateStatus(run, action);

            await _jobService.StopJob(run, action);
        }

        public async Task Run(ObjectId runId, int actionId)
        {
            Assert.MustNotNull(_userLogged.User, "Need to be logged to run script!");

            // checar se ter permissão

            var run = await Node
                .FirstOrDefaultAsync<Run>(a => a.Id == runId);
            Assert.MustNotNull(run, "Run not found! " + runId);

            var control = ActionControl.From(run);
            var effects = control.Run(actionId);
            await ProcessEffects(run, effects);

            await WriteLogInner(run, "Set to run");
        }

        public async Task Stop(ObjectId runId, int actionId)
        {
            Assert.MustNotNull(_userLogged.User, "Need to be logged to run script!");

            // checar se ter permissão

            var run = await Node
                .FirstOrDefaultAsync<Run>(a => a.Id == runId);
            Assert.MustNotNull(run, "Run not found! " + runId);

            var control = ActionControl.From(run);
            var effects = control.Stop(actionId);
            await ProcessEffects(run, effects);

            await WriteLogInner(run, "Set to stop");
        }

        public async Task SetRunning(ObjectId runId, int actionId)
        {
            Assert.MustNotNull(_userLogged.User, "Need to be logged to run script!");

            // checar se ter permissão

            var run = await Node
                .FirstOrDefaultAsync<Run>(a => a.Id == runId);
            Assert.MustNotNull(run, "Run not found! " + runId);

            var control = ActionControl.From(run);
            var effects = control.SetRunning(actionId);
            await ProcessEffects(run, effects);

            var action = control.FindAction(actionId);
            await WriteLogInner(run, $"Action \"{action.Label}\" running...");
        }

        public async Task SetError(ObjectId runId, int actionId, string text)
        {
            Assert.MustNotNull(_userLogged.User, "Need to be logged to error script!");

            // checar se ter permissão

            var run = await Node
                .FirstOrDefaultAsync<Run>(a => a.Id == runId);
            Assert.MustNotNull(run, "Run not found! " + runId);

            var control = ActionControl.From(run);
            var effects = control.SetError(actionId);
            await ProcessEffects(run, effects);

            var action = control.FindAction(actionId);
            await WriteLogInner(run, $"Error on \"{action.Label}\": {text}");
        }

        public async Task SetCompleted(ObjectId runId, int actionId)
        {
            Assert.MustNotNull(_userLogged.User, "Need to be logged to error script!");

            // checar se ter permissão

            var run = await Node
                .FirstOrDefaultAsync<Run>(a => a.Id == runId);
            Assert.MustNotNull(run, "Run not found! " + runId);

            var control = ActionControl.From(run);
            var effects = control.SetCompleted(actionId);
            await ProcessEffects(run, effects);

            var action = control.FindAction(actionId);
            await WriteLogInner(run, $"Action \"{action.Label}\" was completed!");
        }

        public async Task WriteLog(ObjectId runId, string text)
        {
            Assert.MustNotNull(_userLogged.User, "Need to be logged to error script!");

            // checar se ter permissão

            var run = await Node
                .FirstOrDefaultAsync<Run>(a => a.Id == runId);
            Assert.MustNotNull(run, "Run not found! " + runId);

            await WriteLogInner(run, text);
        }

        private async Task WriteLogInner(Run run, string text)
        {
            var update = Builders<Run>.Update
                .Push(r => r.Log, new RunLog
                {
                    Created = DateTime.Now,
                    Text = text
                });

            await Node.UpdateAsync(run, update);
        }

        public async Task SetBreakPoint(ObjectId runId, int actionId)
        {
            Assert.MustNotNull(_userLogged.User, "Need to be logged to error script!");

            // checar se ter permissão

            var run = await Node
                .FirstOrDefaultAsync<Run>(a => a.Id == runId);
            Assert.MustNotNull(run, "Run not found! " + runId);

            var control = ActionControl.From(run);
            var effects = control.SetBreakPoint(actionId);
            await ProcessEffects(run, effects);

            var action = control.FindAction(actionId);
            await WriteLogInner(run, $"BreakPoint on action \"{action.Label}\" setted");
        }

        public async Task CleanBreakPoint(ObjectId runId, int actionId)
        {
            Assert.MustNotNull(_userLogged.User, "Need to be logged to error script!");

            // checar se ter permissão

            var run = await Node
                .FirstOrDefaultAsync<Run>(a => a.Id == runId);
            Assert.MustNotNull(run, "Run not found! " + runId);

            var control = ActionControl.From(run);
            var effects = control.CleanBreakPoint(actionId);
            await ProcessEffects(run, effects);

            var action = control.FindAction(actionId);
            await WriteLogInner(run, $"BreakPoint on action \"{action.Label}\" was cleared");
        }

        public async Task CheckRunState(ObjectId runId)
        {
            Assert.MustNotNull(_userLogged.User, "Need to be logged to run!");

            // checar se ter permissão

            var run = await Node
                .FirstOrDefaultAsync<Run>(a => a.Id == runId);
            Assert.MustNotNull(run, "Run not found! " + runId);

            var actionsStatus = run.Actions
                .Select(a => a.Status)
                .ToList();

            var actualStatus = RunStatus.Waiting;

            var isSomeoneRunning = actionsStatus
                .Any(cs => cs == ActionStatus.Running ||
                    cs == ActionStatus.ToStop ||
                    cs == ActionStatus.ToRun);
            if (isSomeoneRunning)
            {
                actualStatus = RunStatus.Running;
            }
            else
            {
                var isSomeoneError = actionsStatus
                    .Any(cs => cs == ActionStatus.Error);
                if (isSomeoneError)
                {
                    actualStatus = RunStatus.Error;
                }
                else
                {
                    var isAllCompleted = !actionsStatus
                        .Any(ac => ac != ActionStatus.Completed);
                    if (isAllCompleted)
                    {
                        actualStatus = RunStatus.Completed;
                    }
                }
            }


            if (run.Status != actualStatus)
            {
                var logText = string.Empty;
                DateTime? completedDatetime = null; //TODO: fazer update condicional

                run.Status = actualStatus;
                switch (run.Status)
                {
                    case RunStatus.Waiting:
                        logText = "Run is waiting!";
                        break;
                    case RunStatus.Running:
                        logText = "Run started";
                        break;
                    case RunStatus.Completed:
                        logText = "Run completed";
                        completedDatetime = DateTime.Now;
                        break;
                    case RunStatus.Error:
                        logText = "Run with error";
                        break;
                }

                var update = Builders<Run>.Update
                    .Set(r => r.Status, run.Status)
                    .Set(r => r.Completed, completedDatetime)
                    .Push(r => r.Log, new RunLog
                    {
                        Created = DateTime.Now,
                        Text = logText
                    });

                await Node.UpdateAsync(run, update);
            }
        }

        public Task<List<RunList>> ReadRuns(Flow flow)
        {
            Assert.MustNotNull(_userLogged.User, "Need to be logged to error script!");

            // checar se ter permissão

            var project = Builders<Run>.Projection.Expression(r => new RunList
            {
                Id = r.Id,
                Name = r.Name,
                Status = r.Status,
                Completed = r.Completed,
                Created = r.Created
            });
                //.Exclude(r => r.Type)
                //. As<RunList>(); //. .Expression(item => new RunList);

            return Node
                .ProjectToListAsync(a => a.Parent == flow.Id, project);
        }
    }
}
