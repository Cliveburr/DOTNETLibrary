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
                Containers = new List<ActionContainer>(),
                Log = new List<RunLog>()
            };

            run.RootContainerId = MapContainer(run, flow.Root, flow.AgentPool, flow.Tags, true);

            return run;
        }

        private static int MapContainer(Run run, FlowActionContainer flowActionContainer, string agentPool, List<string>? tags, bool isRoot)
        {
            var thisAgentPool = string.IsNullOrEmpty(flowActionContainer.AgentPool) ?
                agentPool :
                flowActionContainer.AgentPool;

            var thisTags = flowActionContainer.Tags == null ?
                tags :
                flowActionContainer.Tags;

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
                MapActions(run, actionContaniner, flowActionContainer.Actions, thisAgentPool, thisTags);
            }
            if (flowActionContainer.Next != null)
            {
                MapNextContainers(run, actionContaniner, flowActionContainer.Next, thisAgentPool, thisTags);
            }

            return actionContaniner.ActionContainerId;
        }

        private static void MapActions(Run run, ActionContainer actionContainer, List<FlowAction> flowActions, string agentPool, List<string>? tags)
        {
            foreach (var flowAction in flowActions)
            {
                var thisAgentPool = string.IsNullOrEmpty(flowAction.AgentPool) ?
                    agentPool :
                    flowAction.AgentPool;

                var thisTags = flowAction.Tags == null ?
                    tags :
                    flowAction.Tags;

                var action = new Action
                {
                    ActionId = run.IdIndexes++,
                    Label = flowAction.Label,
                    AgentPool = thisAgentPool,
                    Tags = thisTags ?? new List<string>(),
                    Status = ActionStatus.Waiting,
                    BreakPoint = false
                };
                run.Actions.Add(action);
                actionContainer.Actions.Add(action.ActionId);
            }
        }

        private static void MapNextContainers(Run run, ActionContainer actionContainer, List<FlowActionContainer> flowActionContainers, string agentPath, List<string>? tags)
        {
            foreach (var flowActionContainer in flowActionContainers)
            {
                var actionContaninerId = MapContainer(run, flowActionContainer, agentPath, tags, false);
                actionContainer.Next.Add(actionContaninerId);
            }
        }
    }
}
