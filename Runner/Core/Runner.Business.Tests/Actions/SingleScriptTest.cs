using Runner.Business.ActionsOutro;
using Runner.Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Tests.Actions
{
    [TestClass]
    public class SingleScriptTest : TestActionsBase2
    {
        protected override ActionControl GetControl()
        {
            var flow = new Flow2
            {
                Name = "Test",
                Root = new FlowAction2
                {
                    Label = "Root",
                    Type = ActionType.Script
                }
            };

            var run = ActionControl.Build(flow);

            return ActionControl.From(run);
        }

        [TestMethod]
        public void RunAndComplete()
        {
            Run("Root")
                .TestCount(1)
                .TestInSequeceActionUpdateToRun();

            SetRunning("Root")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            SetCompleted("Root")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed);
        }

        [TestMethod]
        public void RunAndErrorRetryComplete()
        {
            Run("Root")
                .TestCount(1)
                .TestInSequeceActionUpdateToRun();

            SetRunning("Root")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            SetError("Root")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Error);
            
            Run("Root")
                .TestCount(1)
                .TestInSequeceActionUpdateToRun();

            SetRunning("Root")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            SetCompleted("Root")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed);
        }

        [TestMethod]
        public void RunAndStopContinueAndComplete()
        {
            Run("Root")
                .TestCount(1)
                .TestInSequeceActionUpdateToRun();

            SetRunning("Root")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            Stop("Root")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.ToStop);
            
            SetStopped("Root")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Stopped);

            Run("Root")
                .TestCount(1)
                .TestInSequeceActionUpdateToRun();

            SetRunning("Root")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            SetCompleted("Root")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed);
        }
    }
}
