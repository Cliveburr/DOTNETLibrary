using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Runner.Business.Actions;
using Runner.Business.DataAccess;
using Runner.Business.Datas.Control;
using Runner.Business.Datas.Model;
using Runner.Business.Entities.AgentVersion;
using Runner.Business.Entities.Nodes;
using Runner.Business.Entities.Nodes.Types;
using Runner.Business.Model.Nodes.Types;
using Runner.Business.Model.Table;
using Runner.Business.Security;
using Runner.Business.WatcherNotification;
using System;
using static System.Collections.Specialized.BitVector32;

namespace Runner.Business.Services.NodeTypes
{
    public class RunService : DataServiceBase
    {
        private readonly IdentityProvider _identityProvider;
        private readonly NodeService _nodeService;
        private readonly JobService _jobService;
        private ManualAgentWatcherNotification? _manualAgentWatcherNotification;

        public RunService(Database database, IdentityProvider identityProvider, NodeService nodeService, JobService jobService, IAgentWatcherNotification agentWatcherNotification)
            : base(database)
        {
            _identityProvider = identityProvider;
            _nodeService = nodeService;
            _jobService = jobService;
            _manualAgentWatcherNotification = agentWatcherNotification as ManualAgentWatcherNotification;
        }

        public Task<Run?> ReadByIdStr(string runIdStr)
        {
            if (ObjectId.TryParse(runIdStr, out var runId))
            {
                return ReadById(runId);
            }
            return Task.FromResult<Run?>(null);
        }

        public Task<Run?> ReadById(ObjectId runId)
        {
            return Run
                .FirstOrDefaultAsync(r => r.RunId == runId);
        }

        public Task<List<RunList>> ReadRunsTable(TableRequest request, Flow flow)
        {
            Assert.MustNotNull(_identityProvider.User, "Need to be logged to error script!");

            // checar se ter permissão

            var sort = Builders<Run>.Sort
                .Descending(r => r.Created);

            var project = Builders<Run>.Projection
                .Expression(r => new RunList
                {
                    RunId = r.RunId,
                    Status = r.Status,
                    Completed = r.Completed,
                    Created = r.Created
                });

            return Run.Collection
                .Find(r => r.FlowId == flow.FlowId)
                .Sort(sort)
                .Skip(request.Skip)
                .Limit(request.Take)
                .Project(project)
                .ToListAsync();
        }

        public async Task Delete(ObjectId runId)
        {
            Assert.MustNotNull(_identityProvider.User, "Not logged!");

            await Run
                .DeleteAsync(r => r.RunId == runId);
        }

        public async Task CreateRun(Flow flow, bool setToRun)
        {
            Assert.MustNotNull(_identityProvider.User, "Need to be logged to create app!");

            // checar se ter permissão de Create no parent

            var run = ActionControl.Build(flow);

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

            await Run
                .InsertAsync(run);

            if (setToRun)
            {
                var control = ActionControl.From(run);

                var effects = control.Run(run.RootActionId);

                await ProcessEffects(run, effects);
            }
        }

        private async Task ProcessEffects(Run run, List<CommandEffect> effects)
        {
            var reverseEffects = effects.ToList();
            foreach (var effect in reverseEffects)
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

            foreach (var effect in reverseEffects)
            {
                switch (effect.Type)
                {
                    case ComandEffectType.ActionUpdateStatus: break;
                    case ComandEffectType.ActionUpdateToRun:
                        {
                            await _jobService.QueueRunAction(effect.Action.ActionId, run.RunId);
                            break;
                        }
                    case ComandEffectType.ActionUpdateWithCursor: break;
                    case ComandEffectType.ActionUpdateBreakPoint: break;
                    case ComandEffectType.ActionUpdateToStop:
                        {
                            await _jobService.StopJob(run, effect.Action);
                            break;
                        }
                    default:
                        throw new Exception("Invalid CommandEffect Type: " + effect.Type);
                }
            }

            await CheckRunState(run.RunId);
        }


        private async Task ExecuteActionUpdateToRun(Run run, Actions.Action action)
        {
            await ExecuteActionUpdateStatus(run, action);
        }

        private async Task ExecuteActionUpdateStatus(Run run, Actions.Action action)
        {
            //var filter = Builders<Run>.Filter
            //    .Where(r => r.Id == run.Id && r.Actions.Any(a => a.ActionId == action.ActionId));

            var update = Builders<Run>.Update
                .Set(r => r.Actions.FirstMatchingElement().Status, action.Status)
                .Set(r => r.Actions.FirstMatchingElement().Data, action.Data);

            await Run
                .UpdateAsync(r => r.RunId == run.RunId && r.Actions.Any(a => a.ActionId == action.ActionId), update);

            //_manualAgentWatcherNotification?.InvokeActionUpdated(action);
        }

        private async Task ExecuteActionUpdateWithCursor(Run run, Actions.Action action)
        {
            var update = Builders<Run>.Update
                .Set(r => r.Actions.FirstMatchingElement().WithCursor, action.WithCursor);

            await Run
                .UpdateAsync(r => r.RunId == run.RunId && r.Actions.Any(a => a.ActionId == action.ActionId), update);

            //_manualAgentWatcherNotification?.InvokeActionUpdated(action);
        }

        private async Task ExecuteActionUpdateBreakPoint(Run run, Actions.Action action)
        {
            var update = Builders<Run>.Update
                .Set(r => r.Actions.FirstMatchingElement().BreakPoint, action.BreakPoint);

            await Run
                .UpdateAsync(r => r.RunId == run.RunId && r.Actions.Any(a => a.ActionId == action.ActionId), update);

            //manualAgentWatcherNotification?.InvokeActionUpdated(action);
        }

        private async Task ExecuteActionUpdateToStop(Run run, Actions.Action action)
        {
            await ExecuteActionUpdateStatus(run, action);
        }

        private async Task CheckRunState(ObjectId runId)
        {
            var run = await Run
                .FirstOrDefaultAsync(r => r.RunId == runId);
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
                //var logText = string.Empty;
                DateTime? completedDatetime = null; //TODO: fazer update condicional

                run.Status = actualStatus;
                //switch (run.Status)
                //{
                //    case RunStatus.Waiting:
                //        logText = "Run is waiting!";
                //        break;
                //    case RunStatus.Running:
                //        logText = "Run started";
                //        break;
                //    case RunStatus.Completed:
                //        logText = "Run completed";
                //        completedDatetime = DateTime.Now;
                //        break;
                //    case RunStatus.Error:
                //        logText = "Run stopped with error";
                //        break;
                //}

                run.Completed = completedDatetime;
                //var newLog = new RunLog
                //{
                //    Created = DateTime.Now,
                //    Text = logText
                //};
                //run.Log.Add(newLog);

                var update = Builders<Run>.Update
                    .Set(r => r.Status, run.Status)
                    .Set(r => r.Completed, run.Completed);
                    //.Push(r => r.Log, newLog);

                await Run
                    .UpdateAsync(r => r.RunId == run.RunId, update);
            }
        }

        private async Task WriteLogErrorInner(ObjectId runId, string message, string fullError)
        {
            var update = Builders<Run>.Update
                .Push(r => r.Log, new RunLog
                {
                    Created = DateTime.Now,
                    Text = message,
                    FullError = fullError
                });

            await Run
                .UpdateAsync(r => r.RunId == runId, update);
        }

        private async Task WriteLogInner(ObjectId runId, string text)
        {
            var update = Builders<Run>.Update
                .Push(r => r.Log, new RunLog
                {
                    Created = DateTime.Now,
                    Text = text
                });

            await Run
                .UpdateAsync(r => r.RunId == runId, update);
        }

        public async Task SetRunning(ObjectId runId, int actionId)
        {
            Assert.MustNotNull(_identityProvider.User, "Need to be logged to run script!");

            // checar se ter permissão

            var run = await Run
                .FirstOrDefaultAsync(r => r.RunId == runId);
            Assert.MustNotNull(run, "Run not found! " + runId);

            var control = ActionControl.From(run);
            var effects = control.SetRunning(actionId);
            await ProcessEffects(run, effects);

            var action = control.FindAction(actionId);
            await WriteLogInner(runId, $"Action \"{action.Label}\" is running");

            var actualRun = await ReadById(run.RunId);
            _manualAgentWatcherNotification?.InvokeRunUpdated(actualRun!);
        }

        public async Task SetCompleted(ObjectId runId, int actionId, List<DataProperty>? actionOutput)
        {
            Assert.MustNotNull(_identityProvider.User, "Need to be logged to error script!");

            // checar se ter permissão

            var run = await Run
                .FirstOrDefaultAsync(r => r.RunId == runId);
            Assert.MustNotNull(run, "Run not found! " + runId);

            var control = ActionControl.From(run);
            var effects = control.SetCompleted(actionId);
            if (actionOutput is not null && actionOutput.Any())
            {
                var actionEffect = effects
                    .FirstOrDefault(e => e.Action.ActionId == actionId);
                if (actionEffect is not null)
                {
                    var newData = new DataMerge()
                        .ApplyDataProperty(actionEffect.Action.Data)
                        .ApplyDataProperty(actionOutput)
                        .ToDataProperty();
                    actionEffect.Action.Data = newData;
                }
            }
            await ProcessEffects(run, effects);

            var action = control.FindAction(actionId);
            await WriteLogInner(runId, $"Action \"{action.Label}\" was completed!");

            var actualRun = await ReadById(run.RunId);
            _manualAgentWatcherNotification?.InvokeRunUpdated(actualRun!);
        }

        public async Task SetError(ObjectId runId, int actionId, string message, string fullError)
        {
            Assert.MustNotNull(_identityProvider.User, "Need to be logged to error script!");

            // checar se ter permissão

            var run = await Run
                .FirstOrDefaultAsync(r => r.RunId == runId);
            Assert.MustNotNull(run, "Run not found! " + runId);

            var control = ActionControl.From(run);
            var effects = control.SetError(actionId);
            await ProcessEffects(run, effects);

            var action = control.FindAction(actionId);
            await WriteLogErrorInner(runId, $"Action \"{action.Label}\" error: {message}", fullError);

            var actualRun = await ReadById(run.RunId);
            _manualAgentWatcherNotification?.InvokeRunUpdated(actualRun!);
        }

        public async Task WriteLog(ObjectId runId, string text)
        {
            Assert.MustNotNull(_identityProvider.User, "Need to be logged to error script!");

            // checar se ter permissão

            await WriteLogInner(runId, text);

            var actualRun = await ReadById(runId);
            _manualAgentWatcherNotification?.InvokeRunUpdated(actualRun!);
        }

        public async Task SetRun(ObjectId runId, int actionId)
        {
            Assert.MustNotNull(_identityProvider.User, "Need to be logged to run script!");

            // checar se ter permissão

            var run = await Run
                .FirstOrDefaultAsync(r => r.RunId == runId);
            Assert.MustNotNull(run, "Run not found! " + runId);

            var control = ActionControl.From(run);
            var effects = control.Run(actionId);
            await ProcessEffects(run, effects);

            var actualRun = await ReadById(run.RunId);
            _manualAgentWatcherNotification?.InvokeRunUpdated(actualRun!);
        }

        public async Task Stop(ObjectId runId, int actionId)
        {
            Assert.MustNotNull(_identityProvider.User, "Need to be logged to run script!");

            // checar se ter permissão

            var run = await Run
                .FirstOrDefaultAsync(r => r.RunId == runId);
            Assert.MustNotNull(run, "Run not found! " + runId);

            var control = ActionControl.From(run);
            var effects = control.Stop(actionId);
            await ProcessEffects(run, effects);

            await WriteLogInner(runId, "Set to stop");

            var actualRun = await ReadById(runId);
            _manualAgentWatcherNotification?.InvokeRunUpdated(actualRun!);
        }

        public async Task SetBreakPoint(ObjectId runId, int actionId)
        {
            Assert.MustNotNull(_identityProvider.User, "Need to be logged to error script!");

            // checar se ter permissão

            var run = await Run
                .FirstOrDefaultAsync(r => r.RunId == runId);
            Assert.MustNotNull(run, "Run not found! " + runId);

            var control = ActionControl.From(run);
            var effects = control.SetBreakPoint(actionId);
            await ProcessEffects(run, effects);

            var action = control.FindAction(actionId);
            await WriteLogInner(runId, $"BreakPoint on action \"{action.Label}\" setted");

            var actualRun = await ReadById(runId);
            _manualAgentWatcherNotification?.InvokeRunUpdated(actualRun!);
        }

        public async Task CleanBreakPoint(ObjectId runId, int actionId)
        {
            Assert.MustNotNull(_identityProvider.User, "Need to be logged to error script!");

            // checar se ter permissão

            var run = await Run
                .FirstOrDefaultAsync(r => r.RunId == runId);
            Assert.MustNotNull(run, "Run not found! " + runId);

            var control = ActionControl.From(run);
            var effects = control.CleanBreakPoint(actionId);
            await ProcessEffects(run, effects);

            var action = control.FindAction(actionId);
            await WriteLogInner(runId, $"BreakPoint on action \"{action.Label}\" was cleared");

            var actualRun = await ReadById(runId);
            _manualAgentWatcherNotification?.InvokeRunUpdated(actualRun!);
        }


        public async Task UpdateActionData(ObjectId runId, Actions.Action action)
        {
            Assert.MustNotNull(_identityProvider.User, "Need to be logged to error script!");

            // checar se ter permissão

            var update = Builders<Run>.Update
                .Set(r => r.Actions.FirstMatchingElement().Data, action.Data);

            await Run
                .UpdateAsync(r => r.RunId == runId && r.Actions.Any(a => a.ActionId == action.ActionId), update);
        }
    }
}
