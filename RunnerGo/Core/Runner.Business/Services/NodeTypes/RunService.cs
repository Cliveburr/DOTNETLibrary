using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Runner.Business.Actions;
using Runner.Business.DataAccess;
using Runner.Business.Entities.Nodes.Types;
using Runner.Business.Model.Nodes.Types;
using Runner.Business.Security;
using Runner.Business.WatcherNotification;

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

        public Task<List<RunList>> ReadRuns(Flow flow)
        {
            Assert.MustNotNull(_identityProvider.User, "Need to be logged to error script!");

            // checar se ter permissão

            var project = Builders<Run>.Projection
                .Expression(r => new RunList
                {
                    RunId = r.RunId,
                    Status = r.Status,
                    Completed = r.Completed,
                    Created = r.Created
                });

            return Run
                .ProjectToListAsync(r => r.FlowId == flow.FlowId, project);
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

            await CheckRunState(run.RunId);
            _manualAgentWatcherNotification?.InvokeRunUpdated(run);
        }


        private async Task ExecuteActionUpdateToRun(Run run, Actions.Action action)
        {
            await ExecuteActionUpdateStatus(run, action);

            await _jobService.QueueRunAction(action.ActionId, run.RunId);
        }

        private async Task ExecuteActionUpdateStatus(Run run, Actions.Action action)
        {
            //var filter = Builders<Run>.Filter
            //    .Where(r => r.Id == run.Id && r.Actions.Any(a => a.ActionId == action.ActionId));

            var update = Builders<Run>.Update
                .Set(r => r.Actions.FirstMatchingElement().Status, action.Status);

            await Run
                .UpdateAsync(r => r.RunId == run.RunId && r.Actions.Any(a => a.ActionId == action.ActionId), update);
        }

        private async Task ExecuteActionUpdateWithCursor(Run run, Actions.Action action)
        {
            var update = Builders<Run>.Update
                .Set(r => r.Actions.FirstMatchingElement().WithCursor, action.WithCursor);

            await Run
                .UpdateAsync(r => r.RunId == run.RunId && r.Actions.Any(a => a.ActionId == action.ActionId), update);
        }

        private async Task ExecuteActionUpdateBreakPoint(Run run, Actions.Action action)
        {
            var update = Builders<Run>.Update
                .Set(r => r.Actions.FirstMatchingElement().BreakPoint, action.BreakPoint);

            await Run
                .UpdateAsync(r => r.RunId == run.RunId && r.Actions.Any(a => a.ActionId == action.ActionId), update);
        }

        private async Task ExecuteActionUpdateToStop(Run run, Actions.Action action)
        {
            await ExecuteActionUpdateStatus(run, action);

            await _jobService.StopJob(run, action);
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

                await Run
                    .UpdateAsync(r => r.RunId == run.RunId, update);
            }
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
            await WriteLogInner(runId, $"Action \"{action.Label}\" running...");
        }

        public async Task SetCompleted(ObjectId runId, int actionId)
        {
            Assert.MustNotNull(_identityProvider.User, "Need to be logged to error script!");

            // checar se ter permissão

            var run = await Run
                .FirstOrDefaultAsync(r => r.RunId == runId);
            Assert.MustNotNull(run, "Run not found! " + runId);

            var control = ActionControl.From(run);
            var effects = control.SetCompleted(actionId);
            await ProcessEffects(run, effects);

            var action = control.FindAction(actionId);
            await WriteLogInner(runId, $"Action \"{action.Label}\" was completed!");
        }

        public async Task SetError(ObjectId runId, int actionId, string text)
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
            await WriteLogInner(runId, $"Error on \"{action.Label}\": {text}");
        }

        public async Task WriteLog(ObjectId runId, string text)
        {
            Assert.MustNotNull(_identityProvider.User, "Need to be logged to error script!");

            // checar se ter permissão

            await WriteLogInner(runId, text);
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

            await WriteLogInner(runId, "Set to run");
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
        }
    }
}
