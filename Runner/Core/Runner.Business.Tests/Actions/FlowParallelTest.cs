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
    public class FlowParallelTest : TestActionsBase
    {
        private ActionControl GetControl()
        {
            var flow = new Flow
            {
                Name = "Test",
                Root = new FlowActionContainer
                {
                    Label = "Root",
                    Next = new List<FlowActionContainer>
                    {
                        new FlowActionContainer
                        {
                            Label = "Left",
                            Actions = new List<FlowAction>
                            {
                                new FlowAction
                                {
                                    Label = "1"
                                },
                                new FlowAction
                                {
                                    Label = "2"
                                }
                            }
                        },
                        new FlowActionContainer
                        {
                            Label = "Right",
                            Actions = new List<FlowAction>
                            {
                                new FlowAction
                                {
                                    Label = "3"
                                },
                                new FlowAction
                                {
                                    Label = "4"
                                }
                            }
                        }
                    }
                }
            };

            var run = ActionControl.BuildRun(flow);

            return ActionControl.Set(run);
        }

        // roda uma execução sem erro até o final
        [TestMethod]
        public void SimpleGo()
        {
            var control = GetControl();

            SetRunningEmptyContainer(control, "Root");

            SetRunning(control, "Left");
            SetRunning(control, "Right");

            SetCompletedOnSameContainer(control, "Left");
            SetCompletedOnSameContainer(control, "Right");

            SetRunning(control, "Left");
            SetCompletedAndDone(control, "Left");

            SetRunning(control, "Right");
            SetCompletedAndDone(control, "Right");
        }

        [TestMethod]
        public void LeftErrorAndGo()
        {
            var control = GetControl();

            SetRunningEmptyContainer(control, "Root");

            SetRunning(control, "Left"); // run action 1
            SetRunning(control, "Right"); // run action 3

            SetError(control, "Left"); // error on action 1
            SetCompletedOnSameContainer(control, "Right");  // completed action 3

            SetRunning(control, "Left"); // retry run action 1

            SetRunning(control, "Right"); // run action 4
            SetCompletedAndDone(control, "Right"); // completed action 4

            SetCompletedOnSameContainer(control, "Left"); // completed action 1

            SetRunning(control, "Left"); // run action 2
            SetCompletedAndDone(control, "Left"); // completed action 2
        }

        [TestMethod]
        public void RightBreakPointAndGo()
        {
            var control = GetControl();

            control.Run.Actions[3].BreakPoint = true;

            SetRunningEmptyContainer(control, "Root");

            SetRunning(control, "Left"); // run action 1
            SetRunning(control, "Right"); // run action 3

            SetCompletedAndBreak(control, "Right");  // completed action 3 and break
            SetCompletedOnSameContainer(control, "Left");  // completed action 1

            SetRunning(control, "Left"); // run action 2
            SetRunning(control, "Right"); // run action 4

            SetCompletedAndDone(control, "Left"); // completed action 2
            SetCompletedAndDone(control, "Right"); // completed action 4
        }
    }
}
