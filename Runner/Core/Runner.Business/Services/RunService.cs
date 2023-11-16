using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Runner.Business.Actions;
using Runner.Business.Authentication;
using Runner.Business.DataAccess;
using Runner.Business.Entities;
using Runner.Business.Entities.Agent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Runner.Business.Services
{
    public class RunService : ServiceBase
    {
        private readonly UserLogged _userLogged;
        private readonly JobService _jobService;

        public RunService(Database database, JobService jobService, UserLogged userLogged)
            : base(database)
        {
            _jobService = jobService;
            _userLogged = userLogged;
        }

        public async Task CreateRun(Flow flow)
        {
            Assert.MustNotNull(_userLogged.User, "Need to be logged to create app!");

            // checar se ter permissão de Create no parent

            var run = ActionControl.Build(flow);

            run.Type = NodeType.Run;
            run.Parent = flow.Id;
            run.Status = RunStatus.Waiting;
            run.Created = DateTime.Now;

            // start transaction

            await Node.CreateAsync(run);

            var control = ActionControl.From(run);

            var effects = control.Run(run.RootActionId);

            await ProcessEffects(run, effects);
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

            //TODO: call stop on agent
            //await _jobService.CreateJob(run, action);
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
        }

        public async Task WriteLog(ObjectId runId, string text)
        {
            Assert.MustNotNull(_userLogged.User, "Need to be logged to error script!");

            // checar se ter permissão

            var run = await Node
                .FirstOrDefaultAsync<Run>(a => a.Id == runId);
            Assert.MustNotNull(run, "Run not found! " + runId);
            
            var update = Builders<Run>.Update
                .Push(r => r.Log, new RunLog
                {
                    Created = DateTime.Now,
                    Text = text
                });

            await Node.UpdateAsync(run, update);
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
