using Runner.Business.Actions;
using Runner.Business.Entities;
using Test = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using Action = Runner.Business.Actions.Action;

namespace Runner.Business.Tests.Actions
{
    [TestClass]
    public class SingleTest : TestActionsBase
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
                    }
                },
                Containers = new List<ActionContainer>
                {
                    new ActionContainer
                    {
                        ActionContainerId = 4,
                        Label = "4",
                        Status = ActionContainerStatus.Ready,
                        Position = 0,
                        Actions = new List<int> { 1, 2, 3 },
                        Next = new List<int> { }
                    }
                },
                IdIndexes = 5,
                RootContainerId = 4,
                Log = new List<RunLog>()
            };

            return ActionControl.Set(run);
        }

        // roda uma execução sem erro até o final
        [TestMethod]
        public void SimpleGo()
        {
            var control = GetControl();

            SetRunning(control, 4);
            SetCompletedOnSameContainer(control, 4);

            SetRunning(control, 4);
            SetCompletedOnSameContainer(control, 4);

            SetRunning(control, 4);
            SetCompletedAndDone(control, 4);
        }


        [TestMethod]
        public void ErrorAndGo()
        {
            var control = GetControl();

            SetRunning(control, 4);
            SetCompletedOnSameContainer(control, 4);

            SetRunning(control, 4);
            SetError(control, 4);

            SetRunning(control, 4);
            SetCompletedOnSameContainer(control, 4);

            SetRunning(control, 4);
            SetCompletedAndDone(control, 4);
        }

        [TestMethod]
        public void BreakPointAndGo()
        {
            var control = GetControl();

            control.Run.Actions[1].BreakPoint = true;

            SetRunning(control, 4);
            SetCompletedAndBreak(control, 4);

            SetRunning(control, 4);
            SetCompletedOnSameContainer(control, 4);

            SetRunning(control, 4);
            SetCompletedAndDone(control, 4);
        }
    }
}