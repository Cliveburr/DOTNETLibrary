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
    public class FlowSingleTest : TestActionsBase
    {
        private ActionControl GetControl()
        {
            var flow = new Flow
            {
                Name = "Test",
                Root = new FlowActionContainer
                {
                    Label = "Root",
                    Actions = new List<FlowAction>
                    {
                        new FlowAction
                        {
                            Label = "1"
                        },
                        new FlowAction
                        {
                            Label = "2"
                        },
                        new FlowAction
                        {
                            Label = "3"
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

            SetRunning(control, "Root");
            SetCompletedOnSameContainer(control, "Root");

            SetRunning(control, "Root");
            SetCompletedOnSameContainer(control, "Root");

            SetRunning(control, "Root");
            SetCompletedAndDone(control, "Root");
        }

        [TestMethod]
        public void ErrorAndGo()
        {
            var control = GetControl();

            SetRunning(control, "Root");
            SetCompletedOnSameContainer(control, "Root");

            SetRunning(control, "Root");
            SetError(control, "Root");

            SetRunning(control, "Root");
            SetCompletedOnSameContainer(control, "Root");

            SetRunning(control, "Root");
            SetCompletedAndDone(control, "Root");
        }

        [TestMethod]
        public void BreakPointAndGo()
        {
            var control = GetControl();

            control.Run.Actions[1].BreakPoint = true;

            SetRunning(control, "Root");
            SetCompletedAndBreak(control, "Root");

            SetRunning(control, "Root");
            SetCompletedOnSameContainer(control, "Root");

            SetRunning(control, "Root");
            SetCompletedAndDone(control, "Root");
        }
    }
}
