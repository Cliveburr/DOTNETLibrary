using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using Action = Runner.Business.Actions.Action;
using Runner.Business.Actions;
using Runner.Business.Entities;

namespace Runner.Business.Tests.Actions
{
    [TestClass]
    public class ParallelTest : TestActionsBase
    {
        private ActionControl GetControl()
        {
            var run = new Run
            {
                Name = "Test",
                Actions = new List<Action>
                {
                    new Action
                    {
                        ActionId = 1,
                        Label = "1",
                        AgentPool = "",
                        Tags = new List<string>(),
                        BreakPoint = false,
                        Status = ActionStatus.Waiting
                    },
                    new Action
                    {
                        ActionId = 2,
                        Label = "2",
                        AgentPool = "",
                        Tags = new List<string>(),
                        BreakPoint = false,
                        Status = ActionStatus.Waiting
                    },
                    new Action
                    {
                        ActionId = 3,
                        Label = "3",
                        AgentPool = "",
                        Tags = new List<string>(),
                        BreakPoint = false,
                        Status = ActionStatus.Waiting
                    },
                    new Action
                    {
                        ActionId = 4,
                        Label = "4",
                        AgentPool = "",
                        Tags = new List<string>(),
                        BreakPoint = false,
                        Status = ActionStatus.Waiting
                    }
                },
                Containers = new List<ActionContainer>
                {
                    new ActionContainer
                    {
                        ActionContainerId = 5,
                        Label = "root",
                        Status = ActionContainerStatus.Ready,
                        Position = 0,
                        Actions = new List<int> { },
                        Next = new List<int> { 6, 7 }
                    },
                    new ActionContainer
                    {
                        ActionContainerId = 6,
                        Label = "left",
                        Status = ActionContainerStatus.Waiting,
                        Position = 0,
                        Actions = new List<int> { 1, 2 },
                        Next = new List<int> { }
                    },
                    new ActionContainer
                    {
                        ActionContainerId = 7,
                        Label = "right",
                        Status = ActionContainerStatus.Waiting,
                        Position = 0,
                        Actions = new List<int> { 3, 4 },
                        Next = new List<int> { }
                    }
                },
                IdIndexes = 8,
                RootContainerId = 5,
                Log = new List<RunLog>()
            };

            return ActionControl.Set(run);
        }

        // roda uma execução sem erro até o final
        [TestMethod]
        public void SimpleGo()
        {
            var control = GetControl();

            SetRunningEmptyContainer(control, 5);

            SetRunning(control, 6);
            SetRunning(control, 7);

            SetCompletedOnSameContainer(control, 6);
            SetCompletedOnSameContainer(control, 7);

            SetRunning(control, 6);
            SetCompletedAndDone(control, 6);

            SetRunning(control, 7);
            SetCompletedAndDone(control, 7);
        }

        [TestMethod]
        public void LeftErrorAndGo()
        {
            var control = GetControl();

            SetRunningEmptyContainer(control, 5);

            SetRunning(control, 6); // run action 1
            SetRunning(control, 7); // run action 3

            SetError(control, 6); // error on action 1
            SetCompletedOnSameContainer(control, 7);  // completed action 3

            SetRunning(control, 6); // retry run action 1

            SetRunning(control, 7); // run action 4
            SetCompletedAndDone(control, 7); // completed action 4

            SetCompletedOnSameContainer(control, 6); // completed action 1

            SetRunning(control, 6); // run action 2
            SetCompletedAndDone(control, 6); // completed action 2
        }

        [TestMethod]
        public void RightBreakPointAndGo()
        {
            var control = GetControl();

            control.Run.Actions[3].BreakPoint = true;

            SetRunningEmptyContainer(control, 5);

            SetRunning(control, 6); // run action 1
            SetRunning(control, 7); // run action 3

            SetCompletedAndBreak(control, 7);  // completed action 3 and break
            SetCompletedOnSameContainer(control, 6);  // completed action 1

            SetRunning(control, 6); // run action 2
            SetRunning(control, 7); // run action 4

            SetCompletedAndDone(control, 6); // completed action 2
            SetCompletedAndDone(control, 7); // completed action 4
        }
    }
}
