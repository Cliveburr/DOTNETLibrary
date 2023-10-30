using Runner.Business.Actions;
using Runner.Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using Action = Runner.Business.Actions.Action;

namespace Runner.Business.Tests.Actions
{
    [TestClass]
    public class SequentialTest : TestActionsBase
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
                        Label = "5",
                        Status = ActionContainerStatus.Ready,
                        Position = 0,
                        Actions = new List<int> { 1, 2 },
                        Next = new List<int> { 6 }
                    },
                    new ActionContainer
                    {
                        ActionContainerId = 6,
                        Label = "6",
                        Status = ActionContainerStatus.StandBy,
                        Position = 0,
                        Actions = new List<int> { 3, 4 },
                        Next = new List<int> { }
                    }
                },
                IdIndexes = 7,
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

            SetRunning(control, 5);
            SetCompletedOnSameContainer(control, 5);

            SetRunning(control, 5);
            SetCompletedAndMoveToNextContainer(control, 5);

            SetRunning(control, 6);
            SetCompletedOnSameContainer(control, 6);

            SetRunning(control, 6);
            SetCompletedAndDone(control, 6);
        }

        // roda uma execução com erro, retry e vai até o final
        [TestMethod]
        public void ErrorAndGo()
        {
            var control = GetControl();

            SetRunning(control, 5);
            SetCompletedOnSameContainer(control, 5);

            SetRunning(control, 5);
            SetError(control, 5);

            SetRunning(control, 5);
            SetCompletedAndMoveToNextContainer(control, 5);

            SetRunning(control, 6);
            SetCompletedOnSameContainer(control, 6);

            SetRunning(control, 6);
            SetCompletedAndDone(control, 6);
        }

        // roda uma execução com breakpoint na action 1
        [TestMethod]
        public void BreakPointAndGo()
        {
            var control = GetControl();

            control.Run.Actions[1].BreakPoint = true;

            SetRunning(control, 5);
            SetCompletedAndBreak(control, 5);

            SetRunning(control, 5);
            SetCompletedAndMoveToNextContainer(control, 5);

            SetRunning(control, 6);
            SetCompletedOnSameContainer(control, 6);

            SetRunning(control, 6);
            SetCompletedAndDone(control, 6);
        }


        // roda uma execução com breakpoint na action 1 e 2
        [TestMethod]
        public void DoubleBreakPointAndGo()
        {
            var control = GetControl();

            control.Run.Actions[1].BreakPoint = true;
            control.Run.Actions[2].BreakPoint = true;

            SetRunning(control, 5);
            SetCompletedAndBreak(control, 5);

            SetRunning(control, 5);
            SetCompletedAndMoveToNextContainerWithBreak1(control, 5);

            SetRunning(control, 6);
            SetCompletedOnSameContainer(control, 6);

            SetRunning(control, 6);
            SetCompletedAndDone(control, 6);
        }
    }
}
