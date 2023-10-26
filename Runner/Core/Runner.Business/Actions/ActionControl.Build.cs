using Runner.Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Actions
{
    public partial class ActionControl
    {
        public static ActionControl Set(Run run)
        {
            return new ActionControl(run);
        }

        public static Run BuildRun(Flow flow)
        {
            var run = new Run
            {
                Name = flow.Name,
                IdIndexes = 0,
                Actions = new List<Action>(),
                Containers = new List<ActionContainer>()
            };

            run.RootContainerId = MapContainer(run, flow.Root, flow.AgentPath, true);

            return run;
        }

        private static int MapContainer(Run run, FlowActionContainer flowActionContainer, string agentPath, bool isRoot)
        {
            var thisAgentPath = string.IsNullOrEmpty(flowActionContainer.AgentPath) ?
                agentPath :
                flowActionContainer.AgentPath;

            var actionContaniner = new ActionContainer
            {
                ActionContainerId = run.IdIndexes++,
                Label = flowActionContainer.Label,
                Status = isRoot ? ActionContainerStatus.Ready : ActionContainerStatus.StandBy,
                Position = 0,
                Actions = new List<int>(),
                Next = new List<int>()
            };
            run.Containers.Add(actionContaniner);

            if (flowActionContainer.Actions != null)
            {
                MapActions(run, actionContaniner, flowActionContainer.Actions, thisAgentPath);
            }
            if (flowActionContainer.Next != null)
            {
                MapNextContainers(run, actionContaniner, flowActionContainer.Next, thisAgentPath);
            }

            return actionContaniner.ActionContainerId;
        }

        private static void MapActions(Run run, ActionContainer actionContainer, List<FlowAction> flowActions, string agentPath)
        {
            foreach (var flowAction in flowActions)
            {
                var thisAgentPath = string.IsNullOrEmpty(flowAction.AgentPath) ?
                    agentPath :
                    flowAction.AgentPath;

                var action = new Action
                {
                    ActionId = run.IdIndexes++,
                    Label = flowAction.Label,
                    AgentPath = thisAgentPath,
                    Status = ActionStatus.Waiting,
                    BreakPoint = false
                };
                run.Actions.Add(action);
                actionContainer.Actions.Add(action.ActionId);
            }
        }

        private static void MapNextContainers(Run run, ActionContainer actionContainer, List<FlowActionContainer> flowActionContainers, string agentPath)
        {
            foreach (var flowActionContainer in flowActionContainers)
            {
                var actionContaninerId = MapContainer(run, flowActionContainer, agentPath, false);
                actionContainer.Next.Add(actionContaninerId);
            }
        }
    }
}
