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
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;
using static System.Collections.Specialized.BitVector32;

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

            var run = ActionControl.BuildRun(flow);

            run.Type = NodeType.Run;
            run.Parent = flow.Id;

            // start transaction

            await Node.CreateAsync(run);

            var control = ActionControl.Set(run);

            var effects = control.StartRun();

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
                            Assert.MustNotNull(effect.Action, "Invalid CommandEffect ActionUpdateStatus missing Action");
                            await ExecuteActionUpdateStatus(run, effect.Action);
                            break;
                        }
                    case ComandEffectType.ActionContainerUpdatePosition:
                        {
                            Assert.MustNotNull(effect.ActionContainer, "Invalid CommandEffect ActionContainerUpdatePosition missing ActionContainer");
                            await ExecuteActionContainerUpdatePosition(run, effect.ActionContainer);
                            break;
                        }
                    case ComandEffectType.ActionContainerCreateJobToRun:
                        {
                            Assert.MustNotNull(effect.ActionContainer, "Invalid CommandEffect ActionContainerCreateJobToRun missing ActionContainer");
                            Assert.MustNotNull(effect.Action, "Invalid CommandEffect ActionContainerCreateJobToRun missing Action");
                            await ExecuteActionContainerCreateJobToRun(run, effect.ActionContainer, effect.Action);
                            break;
                        }
                    case ComandEffectType.ActionContainerUpdateStatus:
                        {
                            Assert.MustNotNull(effect.ActionContainer, "Invalid CommandEffect ActionContainerUpdateStatus missing ActionContainer");
                            await ExecuteActionContainerUpdateStatus(run, effect.ActionContainer);
                            break;
                        }
                    case ComandEffectType.ActionContainerUpdatePositionAndStatus:
                        {
                            Assert.MustNotNull(effect.ActionContainer, "Invalid CommandEffect ActionContainerUpdatePositionAndStatus missing ActionContainer");
                            await ExecuteActionContainerUpdatePositionAndStatus(run, effect.ActionContainer);
                            break;
                        }
                    default:
                        throw new Exception("Invalid CommandEffect Type: " + effect.Type);
                }
            }
        }

        private async Task ExecuteActionContainerCreateJobToRun(Run run, ActionContainer actionContainer, Actions.Action action)
        {
            await _jobService.CreateJob(run, actionContainer, action);
        }

        private async Task ExecuteActionUpdateStatus(Run run, Actions.Action action)
        {
            //var filter = Builders<Run>.Filter
            //    .Where(r => r.Id == run.Id && r.Actions.Any(a => a.ActionId == action.ActionId));

            var update = Builders<Run>.Update
                .Set(r => r.Actions.FirstMatchingElement().Status, action.Status);

            await Node.UpdateAsync(r => r.Id == run.Id && r.Actions.Any(a => a.ActionId == action.ActionId), update);
        }

        private async Task ExecuteActionContainerUpdatePosition(Run run, ActionContainer actionContainer)
        {
            var update = Builders<Run>.Update
                .Set(a => a.Containers.FirstMatchingElement().Position, actionContainer.Position);

            await Node.UpdateAsync(r => r.Id == run.Id && r.Containers
                .Any(c => c.ActionContainerId == actionContainer.ActionContainerId), update);
        }

        private async Task ExecuteActionContainerUpdateStatus(Run run, ActionContainer actionContainer)
        {
            var update = Builders<Run>.Update
                .Set(a => a.Containers.FirstMatchingElement().Status, actionContainer.Status);

            await Node.UpdateAsync(r => r.Id == run.Id && r.Containers
                .Any(c => c.ActionContainerId == actionContainer.ActionContainerId), update);
        }

        private async Task ExecuteActionContainerUpdatePositionAndStatus(Run run, ActionContainer actionContainer)
        {
            var update = Builders<Run>.Update
                .Set(a => a.Containers.FirstMatchingElement().Status, actionContainer.Status)
                .Set(a => a.Containers[-1].Position, actionContainer.Position);

            await Node.UpdateAsync(r => r.Id == run.Id && r.Containers
                .Any(c => c.ActionContainerId == actionContainer.ActionContainerId), update);
        }

        public async Task SetRunning(ObjectId runId, int actionContainerId)
        {
            Assert.MustNotNull(_userLogged.User, "Need to be logged to run script!");

            // checar se ter permissão

            var run = await Node
                .FirstOrDefaultAsync<Run>(a => a.Id == runId);
            Assert.MustNotNull(run, "Run not found! " + runId);

            var control = ActionControl.Set(run);

            var effects = control.SetRunning(actionContainerId);

            await ProcessEffects(run, effects);
        }

        public async Task SetError(ObjectId runId, int actionContainerId, string text)
        {
            Assert.MustNotNull(_userLogged.User, "Need to be logged to error script!");

            // checar se ter permissão

            var run = await Node
                .FirstOrDefaultAsync<Run>(a => a.Id == runId);
            Assert.MustNotNull(run, "Run not found! " + runId);

            var control = ActionControl.Set(run);

            var effects = control.SetError(actionContainerId);

            await ProcessEffects(run, effects);
        }

        public async Task SetCompleted(ObjectId runId, int actionContainerId)
        {
            Assert.MustNotNull(_userLogged.User, "Need to be logged to error script!");

            // checar se ter permissão

            var run = await Node
                .FirstOrDefaultAsync<Run>(a => a.Id == runId);
            Assert.MustNotNull(run, "Run not found! " + runId);

            var control = ActionControl.Set(run);

            var effects = control.SetCompleted(actionContainerId);

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
    }
}
