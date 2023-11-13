using Runner.Business.ActionsOutro;
using Runner.Business.Entities;
using Runner.Business.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Tests.Actions
{
    [TestClass]
    public class SingleScriptTest : TestActionsBase
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
                .HasActionUpdateToRun("Root")
                .IsCheckedAll();

            SetRunning("Root")
                .HasActionUpdateRunning("Root")
                .IsCheckedAll();

            SetCompleted("Root")
                .HasActionUpdateCompleted("Root")
                .HasActionClearingCursor("Root")
                .IsCheckedAll();
        }

        [TestMethod]
        public void RunAndErrorRetryComplete()
        {
            Run("Root")
                .HasActionUpdateToRun("Root")
                .IsCheckedAll();

            SetRunning("Root")
                .HasActionUpdateRunning("Root")
                .IsCheckedAll();

            SetError("Root")
                .HasActionUpdateError("Root")
                .IsCheckedAll();

            Run("Root")
                .HasActionUpdateToRun("Root")
                .IsCheckedAll();

            SetRunning("Root")
                .HasActionUpdateRunning("Root")
                .IsCheckedAll();

            SetCompleted("Root")
                .HasActionUpdateCompleted("Root")
                .HasActionClearingCursor("Root")
                .IsCheckedAll();
        }

        [TestMethod]
        public void RunAndStopContinueAndComplete()
        {
            Run("Root")
                .HasActionUpdateToRun("Root")
                .IsCheckedAll();

            SetRunning("Root")
                .HasActionUpdateRunning("Root")
                .IsCheckedAll();

            Stop("Root")
                .HasActionUpdateToStop("Root")
                .IsCheckedAll();

            SetStopped("Root")
                .HasActionUpdateStopped("Root")
                .IsCheckedAll();

            Run("Root")
                .HasActionUpdateToRun("Root")
                .IsCheckedAll();

            SetRunning("Root")
                .HasActionUpdateRunning("Root")
                .IsCheckedAll();

            SetCompleted("Root")
                .HasActionUpdateCompleted("Root")
                .HasActionClearingCursor("Root")
                .IsCheckedAll();
        }
    }
}
