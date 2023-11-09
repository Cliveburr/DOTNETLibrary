using Runner.Business.Actions;
using Runner.Business.ActionsOutro.Types;
using Runner.Business.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.ActionsOutro
{
    public partial class ActionControl
    {
        public Run2 EntityRun { get; private set; }

        private ActionControl(Run2 run)
        {
            EntityRun = run;
        }

        public static ActionControl From(Run2 run)
        {
            return new ActionControl(run);
        }

        public static Run2 Build(Flow2 flow)
        {
            var run = new Run2
            {
                Name = flow.Name,
                IdIndexes = 0,
                Actions = new List<Action>(),
                Cursors = new List<Cursor>(),
                Log = new List<RunLog>()
            };

            run.RootActionId = MapAction(run, flow.Root, null);
            
            run.Cursors.Add(new Cursor
            {
                ActionId = run.RootActionId,
                ActionsPasseds = new List<int>()
            });

            return run;
        }

        private static int MapAction(Run2 run, FlowAction2 flowAction, ActionsOutro.Action? parent)
        {
            var thisAgentPool = !string.IsNullOrEmpty(flowAction.AgentPool) ?
                flowAction.AgentPool :
                parent?.AgentPool;

            var thisTags = flowAction.Tags != null ?
                flowAction.Tags :
                parent?.Tags;

            var newActionFlow = new ActionsOutro.Action
            {
                ActionId = run.IdIndexes++,
                Label = flowAction.Label,
                AgentPool = thisAgentPool,
                Tags = thisTags,
                Status = ActionsOutro.ActionStatus.Waiting,
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
