using Runner.Business.Actions;
using Runner.Business.Authentication;
using Runner.Business.DataAccess;
using Runner.Business.Entities;
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

            var run = ActionControl.BuildRun(flow);

            run.Type = NodeType.Run;
            run.Parent = flow.Id;

            // start transaction

            await Node.CreateAsync(run);

            var control = ActionControl.Set(run);

            var effects = control.StartRun(run.RootContainerId);

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
                            break;
                        }
                    case ComandEffectType.ActionContainerUpdatePosition:
                        {
                            break;
                        }
                    case ComandEffectType.ActionCreateJobToRun:
                        {
                            Assert.MustNotNull(effect.Action, "Invalid CommandEffect missing Action");
                            await ExecuteCreateJobToRun(run, effect.Action);
                            break;
                        }
                    case ComandEffectType.ActionContainerUpdateStatus:
                        {
                            break;
                        }
                    case ComandEffectType.ActionContainerUpdatePositionAndStatus:
                        {
                            break;
                        }
                    default:
                        throw new Exception("Invalid CommandEffect Type: " + effect.Type);
                }
            }
        }

        private async Task ExecuteCreateJobToRun(Run run, Actions.Action action)
        {
            await _jobService.CreateJob(run, action);
        }
    }
}
