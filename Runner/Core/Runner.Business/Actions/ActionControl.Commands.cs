using MongoDB.Driver;
using Runner.Business.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Runner.Business.Actions
{
    public partial class ActionControl
    {
        public Run Run { get; private set; }

        private ActionControl(Run run)
        {
            Run = run;
        }

        public List<CommandEffect> SetRunning(int actionContainerId)
        {
            // buscar
            var (container, action) = FindPositionActionOnContainer(actionContainerId);

            // checagem container
            Assert.Enum.In(container.Status,
                ActionContainerStatus.Ready,
                $"Container is not ready to run! {container.ActionContainerId}-{container.Label}");

            // checagem action
            Assert.Enum.In(action.Status, new[] {
                ActionStatus.Waiting,
                ActionStatus.Stopped,
                ActionStatus.Error
            }, $"Action in wrong status to run! {action.ActionId}-{action.Label}");

            // mudança
            action.Status = ActionStatus.Running;

            return new List<CommandEffect>
            {
                new CommandEffect(ComandEffectType.ActionUpdateStatus, action)
            };
        }

        public List<CommandEffect> SetError(int actionContainerId)
        {
            // buscar
            var (container, action) = FindPositionActionOnContainer(actionContainerId);

            // checagem container
            Assert.Enum.In(container.Status,
                ActionContainerStatus.Ready,
                $"Container is not ready to error! {container.ActionContainerId}-{container.Label}");

            // checagem action
            Assert.Enum.In(action.Status,
                ActionStatus.Running,
                $"Action is not running to error! {action.ActionId}-{action.Label}");

            // mudança
            action.Status = ActionStatus.Error;

            return new List<CommandEffect>
            {
                new CommandEffect(ComandEffectType.ActionUpdateStatus, action)
            };
        }

        public List<CommandEffect> SetStopped(int actionContainerId)
        {
            // buscar
            var (container, action) = FindPositionActionOnContainer(actionContainerId);

            // checagem cursor
            Assert.Enum.In(container.Status,
                ActionContainerStatus.Ready,
                $"Container is not ready to stop! {container.ActionContainerId}-{container.Label}");

            // checagem action
            Assert.Enum.In(action.Status,
                ActionStatus.Running,
                $"Action is not running to stop! {action.ActionId}-{action.Label}");

            // mudança
            action.Status = ActionStatus.Stopped;

            return new List<CommandEffect>
            {
                new CommandEffect(ComandEffectType.ActionUpdateStatus, action)
            };
        }

        public List<CommandEffect> SetCompleted(int actionContainerId)
        {
            var effects = new List<CommandEffect>();

            // buscar
            var (container, action) = FindPositionActionOnContainer(actionContainerId);

            // checagem cursor
            Assert.Enum.In(container.Status,
                ActionContainerStatus.Ready,
                $"Container is not ready to complete! {container.ActionContainerId}-{container.Label}");

            // checagem action
            Assert.Enum.In(action.Status,
                ActionStatus.Running,
                $"Action is not running to complete! {action.ActionId}-{action.Label}");

            // mudança
            action.Status = ActionStatus.Completed;
            effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, action));

            // avança forçando para rodar todas actions a frente
            var nextPosition = container.Position + 1;
            if (nextPosition < container.Actions.Count)
            {
                // mudança
                container.Position = nextPosition;
                effects.Add(new CommandEffect(ComandEffectType.ActionContainerUpdatePosition, container));

                var nextAction = FindPositionAction(container);
                if (!nextAction.BreakPoint)
                {
                    // create job to run
                    effects.Add(new CommandEffect(ComandEffectType.ActionCreateJobToRun, nextAction));
                }
            }
            else
            {
                // mudança
                container.Status = ActionContainerStatus.Done;
                effects.Add(new CommandEffect(ComandEffectType.ActionContainerUpdateStatus, container));

                foreach (var nextContainerId in container.Next)
                {
                    var nextContainer = FindContainer(nextContainerId);

                    // mudança
                    nextContainer.Position = 0;
                    nextContainer.Status = ActionContainerStatus.Ready;
                    effects.Add(new CommandEffect(ComandEffectType.ActionContainerUpdatePositionAndStatus, nextContainer));

                    var nextAction = FindPositionAction(container);
                    if (!nextAction.BreakPoint)
                    {
                        // create job to run
                        effects.Add(new CommandEffect(ComandEffectType.ActionCreateJobToRun, nextAction));
                    }
                }
            }

            return effects;
        }
    }
}
