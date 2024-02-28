using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Runner.Business.Actions;
using Runner.Business.DataAccess;
using Runner.Business.Datas.Model;
using Runner.Business.Datas.Object;
using Runner.Business.Entities.Nodes.Types;
using Runner.Business.Model.Nodes.Types;
using Runner.Business.Model.Table;
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

        public Task<Run> CreateRun(ObjectId flowId, List<DataProperty>? input, bool setToRun)
        {
            return CreateRun(flowId, input, setToRun, null);
        }

        private async Task<Run> CreateRun(ObjectId flowId, List<DataProperty>? input, bool setToRun, FromParentRun? fromParentRun)
        {
            var flow = await Flow
                .FirstOrDefaultAsync(f => f.FlowId == flowId);
            Assert.MustNotNull(flow, $"Flow not found! FlowId: {flowId}");

            return await CreateRun(flow, input, setToRun, fromParentRun);
        }

        public Task<Run> CreateRun(Flow flow, List<DataProperty>? input, bool setToRun)
        {
            return CreateRun(flow, input, setToRun, null);
        }

        private async Task<Run> CreateRun(Flow flow, List<DataProperty>? input, bool setToRun, FromParentRun? fromParentRun)
        {
            Assert.MustNotNull(_identityProvider.User, "Need to be logged to create app!");

            // checar se ter permissão de Create no parent

            var run = ActionControl.Build(flow);

            run.Status = RunStatus.Waiting;
            run.Created = DateTime.Now;
            run.Input = input;
            run.FromParentRun = fromParentRun;
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

            return run;
        }

        private async Task ProcessEffects(Run run, List<CommandEffect> effects)
        {
            var reverseEffects = effects.ToList();
            foreach (var effect in reverseEffects)
            {
                switch (effect.Type)
                {
                    case ComandEffectType.ActionUpdateStatus:
                    case ComandEffectType.ActionUpdateToRun:
                    case ComandEffectType.ActionUpdateParentRunToRun:
                    case ComandEffectType.ActionUpdateToStop:
                    case ComandEffectType.ActionUpdateParentRunToStop:
                        {
                            await ExecuteActionUpdateStatus(run, effect.Action);
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
                    default:
                        throw new Exception("Invalid CommandEffect Type: " + effect.Type);
                }
            }

            foreach (var effect in reverseEffects)
            {
                switch (effect.Type)
                {
                    case ComandEffectType.ActionUpdateStatus: break;
                    case ComandEffectType.ActionUpdateWithCursor: break;
                    case ComandEffectType.ActionUpdateBreakPoint: break;
                    case ComandEffectType.ActionUpdateToRun:
                        {
                            await _jobService.QueueRunScript(effect.Action.ActionId, run.RunId);
                            break;
                        }
                    case ComandEffectType.ActionUpdateToStop:
                        {
                            await _jobService.QueueStopScript(effect.Action.ActionId, run.RunId);
                            break;
                        }
                    case ComandEffectType.ActionUpdateParentRunToRun:
                        {
                            await ExecuteActionUpdateParentRunToRun(run, effect.Action);
                            break;
                        }
                    default:
                        throw new Exception("Invalid CommandEffect Type: " + effect.Type);
                }
            }

            await CheckRunState(run.RunId);
        }

        public async Task ExecuteActionUpdateParentRunToRun(Run run, Actions.Action action)
        {
            try
            {
                if (action.ParentRunId is null)
                {
                    var data = new DataObject(action.Data);
                    var flowNodeId = data.ReadNodePath("Flow");
                    Assert.MustNotNull(flowNodeId, $"Missing Flow data in action: {action.Label}");

                    var flow = await Flow
                        .FirstOrDefaultAsync(f => f.NodeId == flowNodeId);
                    Assert.MustNotNull(flow, $"Flow not found! FlowId: {flowNodeId}");

                    var fromParentRun = new FromParentRun
                    {
                        RunId = run.RunId,
                        ActionId = action.ActionId
                    };
                    var parentRun = await CreateRun(flow, action.Data, true, fromParentRun);

                    action.ParentRunId = parentRun.RunId;

                    var update = Builders<Run>.Update
                        .Set(r => r.Actions.FirstMatchingElement().ParentRunId, action.ParentRunId);

                    await Run
                        .UpdateAsync(r => r.RunId == run.RunId && r.Actions.Any(a => a.ActionId == action.ActionId), update);
                }
                else
                {
                    await SetRun(action.ParentRunId.Value);
                }
            }
            catch (Exception ex)
            {
                await SetError(run.RunId, action.ActionId, ex.Message, ex.ToString());
            }
        }

        private async Task ExecuteActionUpdateStatus(Run run, Actions.Action action)
        {
            var update = Builders<Run>.Update
                .Set(r => r.Actions.FirstMatchingElement().Status, action.Status)
                .Set(r => r.Actions.FirstMatchingElement().Data, action.Data);

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
                run.Status = actualStatus;

                var update = Builders<Run>.Update
                    .Set(r => r.Status, run.Status);

                if (run.Status == RunStatus.Completed)
                {
                    run.Completed = DateTime.Now;

                    update = update
                        .Set(r => r.Completed, run.Completed);
                }

                await Run
                    .UpdateAsync(r => r.RunId == run.RunId, update);

                if (run.FromParentRun is not null)
                {
                    await UpdateFromParentRun(run.Status, run.FromParentRun);
                }
            }
        }

        private async Task UpdateFromParentRun(RunStatus status, FromParentRun fromParentRun)
        {
            switch (status)
            {
                case RunStatus.Running:
                    {
                        await SetRunning(fromParentRun.RunId, fromParentRun.ActionId);
                        break;
                    }
                case RunStatus.Error:
                    {
                        await SetError(fromParentRun.RunId, fromParentRun.ActionId, "ParentRun with error!", "ParentRun with error!");
                        break;
                    }
                case RunStatus.Completed:
                    {
                        await SetCompleted(fromParentRun.RunId, fromParentRun.ActionId, null); //TODO: montar outputData
                        break;
                    }
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

        private Task WriteLogInner(ObjectId runId, string text)
        {
            var update = Builders<Run>.Update
                .Push(r => r.Log, new RunLog
                {
                    Created = DateTime.Now,
                    Text = text
                });

            return Run
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
                    var newData = new DataObject()
                        .Merge(actionEffect.Action.Data)
                        .Merge(actionOutput)
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

        public async Task SetRun(ObjectId runId)
        {
            Assert.MustNotNull(_identityProvider.User, "Need to be logged to run script!");

            // checar se ter permissão

            var run = await Run
                .FirstOrDefaultAsync(r => r.RunId == runId);
            Assert.MustNotNull(run, "Run not found! " + runId);

            var control = ActionControl.From(run);
            var effects = control.Run();
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
