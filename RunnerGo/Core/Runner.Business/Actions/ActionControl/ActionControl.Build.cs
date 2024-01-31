using MongoDB.Bson;
using Runner.Business.Entities.Nodes.Types;

namespace Runner.Business.Actions
{
    public partial class ActionControl
    {
        public Run EntityRun { get; private set; }

        private ActionControl(Run run)
        {
            EntityRun = run;
        }

        public static ActionControl From(Run run)
        {
            return new ActionControl(run);
        }

        public static Run Build(Flow flow)
        {
            var run = new Run
            {
                FlowId = flow.FlowId,
                IdIndexes = 0,
                Actions = new List<Action>(),
                Log = new List<RunLog>()
            };

            run.RootActionId = MapAction(run, flow.Root, null);
            
            if (run.Actions.Any())
            {
                run.Actions[0].WithCursor = true;
            }

            return run;
        }

        private static int MapAction(Run run, FlowAction flowAction, Action? parent)
        {
            var thisAgentPool = !string.IsNullOrEmpty(flowAction.AgentPool) ?
                flowAction.AgentPool :
                parent?.AgentPool;

            var thisTags = flowAction.Tags != null ?
                flowAction.Tags :
                parent?.Tags;

            var newActionFlow = new Action
            {
                ActionId = run.IdIndexes++,
                Label = flowAction.Label,
                AgentPool = thisAgentPool,
                Tags = thisTags,
                Status = ActionStatus.Waiting,
                Type = flowAction.Type,
                BreakPoint = false,
                Parent = parent?.ActionId
            };
            run.Actions.Add(newActionFlow);

            if (flowAction.Childs != null)
            {
                newActionFlow.Childs = flowAction.Childs
                    .Select(c => MapAction(run, c, newActionFlow))
                    .ToList();
            }

            return newActionFlow.ActionId;
        }
    }
}
