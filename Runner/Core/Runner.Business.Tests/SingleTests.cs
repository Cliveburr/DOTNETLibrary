using Runner.Business.Actions;
using Runner.Business.Entities;
using Test = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Runner.Business.Tests
{
    [TestClass]
    public class SingleTests
    {
        private ActionControl GetControl()
        {
            var run = new Run
            {
                Name = "Test",
                Actions = new List<Actions.Action>
                {
                    new Actions.Action
                    {
                        ActionId = 1,
                        Label = "1",
                        BreakPoint = false,
                        Status = ActionStatus.Waiting
                    },
                    new Actions.Action
                    {
                        ActionId = 2,
                        Label = "2",
                        BreakPoint = false,
                        Status = ActionStatus.Waiting
                    },
                    new Actions.Action
                    {
                        ActionId = 3,
                        Label = "3",
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
                RootContainerId = 4
            };

            return ActionControl.Build(run);
        }

        // roda uma execução sem erro até o final
        [TestMethod]
        public void SimpleGo()
        {
            var control = GetControl();

            for (var i = 0; i < 2; i++)
            {
                var setRunning = control.SetRunning(4);
                Test.AreEqual(setRunning[0].Action!.Status, ActionStatus.Running);
                Test.AreEqual(setRunning[0].Type, ComandEffectType.ActionUpdateStatus);

                var setCompleted = control.SetCompleted(4);
                Test.AreEqual(setCompleted[0].Action!.Status, ActionStatus.Completed);
                Test.AreEqual(setCompleted[0].Type, ComandEffectType.ActionUpdateStatus);
                Test.AreEqual(setCompleted[1].Type, ComandEffectType.ActionContainerUpdatePosition);
                Test.AreEqual(setCompleted[2].Type, ComandEffectType.ActionCreateJobToRun);
            }

            var setRunningFinal = control.SetRunning(4);
            Test.AreEqual(setRunningFinal[0].Action!.Status, ActionStatus.Running);
            Test.AreEqual(setRunningFinal[0].Type, ComandEffectType.ActionUpdateStatus);

            var setCompletedFinal = control.SetCompleted(4);
            Test.AreEqual(setCompletedFinal[0].Action!.Status, ActionStatus.Completed);
            Test.AreEqual(setCompletedFinal[0].Type, ComandEffectType.ActionUpdateStatus);
            Test.AreEqual(setCompletedFinal[1].Type, ComandEffectType.ActionContainerUpdateStatus);
            Test.AreEqual(setCompletedFinal[1].ActionContainer!.Status, ActionContainerStatus.Done);
        }
    }
}