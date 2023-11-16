using Runner.Business.Actions;
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
    public class SingleParallelTest : TestActionsBase
    {
        protected override ActionControl GetControl()
        {
            var flow = new Flow
            {
                Name = "Test",
                Root = new FlowAction
                {
                    Label = "Parallel",
                    Type = ActionType.Parallel,
                    Childs = new List<FlowAction>
                    {
                        new FlowAction
                        {
                            Label = "One",
                            Type = ActionType.Script
                        },
                        new FlowAction
                        {
                            Label = "Two",
                            Type = ActionType.Script
                        }
                    }
                }
            };

            var run = ActionControl.Build(flow);

            return ActionControl.From(run);
        }

        [TestMethod]
        public void RunAndComplete()
        {
            /*
                Parallel = Waiting, Cursor
                    One = Waiting
                    Two = Waiting
            */

            Run("Parallel")
                .HasActionClearingCursor("Parallel")
                .HasActionUpdateRunning("Parallel")
                .HasActionSettingCursor("One")
                .HasActionUpdateToRun("One")
                .HasActionSettingCursor("Two")
                .HasActionUpdateToRun("Two")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = ToRun, Cursor
                    Two = ToRun, Cursor
            */

            SetRunning("One")
                .HasActionUpdateRunning("One")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = Running, Cursor
                    Two = ToRun, Cursor
            */

            SetRunning("Two")
                .HasActionUpdateRunning("Two")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = Running, Cursor
                    Two = Running, Cursor
            */

            SetCompleted("One")
                .HasActionUpdateCompleted("One")
                .HasActionClearingCursor("One")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = Completed
                    Two = Running, Cursor
            */

            SetCompleted("Two")
                .HasActionUpdateCompleted("Two")
                .HasActionClearingCursor("Two")
                .HasActionUpdateCompleted("Parallel")
                .IsCheckedAll();

            /*
                Parallel = Completed
                    One = Completed
                    Two = Completed
            */

            CheckAllCompleted();
        }

        [TestMethod]
        public void ErrorOnOneRetryComplete()
        {
            /*
                Parallel = Waiting, Cursor
                    One = Waiting
                    Two = Waiting
            */

            Run("Parallel")
                .HasActionClearingCursor("Parallel")
                .HasActionUpdateRunning("Parallel")
                .HasActionSettingCursor("One")
                .HasActionUpdateToRun("One")
                .HasActionSettingCursor("Two")
                .HasActionUpdateToRun("Two")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = ToRun, Cursor
                    Two = ToRun, Cursor
            */

            SetRunning("One")
                .HasActionUpdateRunning("One")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = Running, Cursor
                    Two = ToRun, Cursor
            */

            SetRunning("Two")
                .HasActionUpdateRunning("Two")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = Running, Cursor
                    Two = Running, Cursor
            */

            SetError("One")
                .HasActionUpdateError("One")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = Error, Cursor
                    Two = Running, Cursor
            */

            SetCompleted("Two")
                .HasActionUpdateCompleted("Two")
                .HasActionClearingCursor("Two")
                .HasActionUpdateError("Parallel")
                .IsCheckedAll();

            /*
                Parallel = Error
                    One = Error, Cursor
                    Two = Completed
            */

            Run("One")
                .HasActionUpdateToRun("One")
                .HasActionUpdateRunning("Parallel")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = ToRun, Cursor
                    Two = Completed
            */


            SetRunning("One")
                .HasActionUpdateRunning("One")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = Running, Cursor
                    Two = Completed
            */

            SetCompleted("One")
                .HasActionUpdateCompleted("One")
                .HasActionClearingCursor("One")
                .HasActionUpdateCompleted("Parallel")
                .IsCheckedAll();

            /*
                Parallel = Completed
                    One = Completed
                    Two = Completed
            */

            CheckAllCompleted();
        }

        [TestMethod]
        public void ErrorOnTwoRetryComplete()
        {
            /*
                Parallel = Waiting, Cursor
                    One = Waiting
                    Two = Waiting
            */

            Run("Parallel")
                .HasActionClearingCursor("Parallel")
                .HasActionUpdateRunning("Parallel")
                .HasActionSettingCursor("One")
                .HasActionUpdateToRun("One")
                .HasActionSettingCursor("Two")
                .HasActionUpdateToRun("Two")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = ToRun, Cursor
                    Two = ToRun, Cursor
            */

            SetRunning("One")
                .HasActionUpdateRunning("One")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = Running, Cursor
                    Two = ToRun, Cursor
            */

            SetRunning("Two")
                .HasActionUpdateRunning("Two")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = Running, Cursor
                    Two = Running, Cursor
            */

            SetCompleted("One")
                .HasActionUpdateCompleted("One")
                .HasActionClearingCursor("One")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = Completed
                    Two = Running, Cursor
            */

            SetError("Two")
                .HasActionUpdateError("Two")
                .HasActionUpdateError("Parallel")
                .IsCheckedAll();

            /*
                Parallel = Error
                    One = Completed
                    Two = Error, Cursor
            */

            Run("Two")
                .HasActionUpdateToRun("Two")
                .HasActionUpdateRunning("Parallel")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = Completed
                    Two = ToRun, Cursor
            */

            SetRunning("Two")
                .HasActionUpdateRunning("Two")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = Completed
                    Two = Running, Cursor
            */

            SetCompleted("Two")
                .HasActionUpdateCompleted("Two")
                .HasActionClearingCursor("Two")
                .HasActionUpdateCompleted("Parallel")
                .IsCheckedAll();

            /*
                Parallel = Completed
                    One = Completed
                    Two = Completed
            */

            CheckAllCompleted();
        }

        [TestMethod]
        public void StopOnOneRetryComplete()
        {
            /*
                Parallel = Waiting, Cursor
                    One = Waiting
                    Two = Waiting
            */

            Run("Parallel")
                .HasActionClearingCursor("Parallel")
                .HasActionUpdateRunning("Parallel")
                .HasActionSettingCursor("One")
                .HasActionUpdateToRun("One")
                .HasActionSettingCursor("Two")
                .HasActionUpdateToRun("Two")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = ToRun, Cursor
                    Two = ToRun, Cursor
            */

            SetRunning("One")
                .HasActionUpdateRunning("One")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = Running, Cursor
                    Two = ToRun, Cursor
            */

            SetRunning("Two")
                .HasActionUpdateRunning("Two")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = Running, Cursor
                    Two = Running, Cursor
            */

            Stop("One")
                .HasActionUpdateToStop("One")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = ToStop, Cursor
                    Two = Running, Cursor
            */

            SetStopped("One")
                .HasActionUpdateStopped("One")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = Stopped, Cursor
                    Two = Running, Cursor
            */

            SetCompleted("Two")
                .HasActionUpdateCompleted("Two")
                .HasActionClearingCursor("Two")
                .HasActionUpdateStopped("Parallel")
                .IsCheckedAll();

            /*
                Parallel = Stopped
                    One = Stopped, Cursor
                    Two = Completed
            */

            Run("One")
                .HasActionUpdateToRun("One")
                .HasActionUpdateRunning("Parallel")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = ToRun, Cursor
                    Two = Completed
            */


            SetRunning("One")
                .HasActionUpdateRunning("One")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = Running, Cursor
                    Two = Completed
            */

            SetCompleted("One")
                .HasActionUpdateCompleted("One")
                .HasActionClearingCursor("One")
                .HasActionUpdateCompleted("Parallel")
                .IsCheckedAll();

            /*
                Parallel = Completed
                    One = Completed
                    Two = Completed
            */

            CheckAllCompleted();
        }

        [TestMethod]
        public void StopOnTwoRetryComplete()
        {
            /*
                Parallel = Waiting, Cursor
                    One = Waiting
                    Two = Waiting
            */

            Run("Parallel")
                .HasActionClearingCursor("Parallel")
                .HasActionUpdateRunning("Parallel")
                .HasActionSettingCursor("One")
                .HasActionUpdateToRun("One")
                .HasActionSettingCursor("Two")
                .HasActionUpdateToRun("Two")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = ToRun, Cursor
                    Two = ToRun, Cursor
            */

            SetRunning("One")
                .HasActionUpdateRunning("One")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = Running, Cursor
                    Two = ToRun, Cursor
            */

            SetRunning("Two")
                .HasActionUpdateRunning("Two")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = Running, Cursor
                    Two = Running, Cursor
            */

            SetCompleted("One")
                .HasActionUpdateCompleted("One")
                .HasActionClearingCursor("One")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = Completed
                    Two = Running, Cursor
            */

            Stop("Two")
                .HasActionUpdateToStop("Two")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = Completed
                    Two = ToStop, Cursor
            */

            SetStopped("Two")
                .HasActionUpdateStopped("Two")
                .HasActionUpdateStopped("Parallel")
                .IsCheckedAll();

            /*
                Parallel = Stopped
                    One = Completed
                    Two = Stopped, Cursor
            */

            Run("Two")
                .HasActionUpdateToRun("Two")
                .HasActionUpdateRunning("Parallel")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = Completed
                    Two = ToRun, Cursor
            */

            SetRunning("Two")
                .HasActionUpdateRunning("Two")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = Completed
                    Two = Running, Cursor
            */

            SetCompleted("Two")
                .HasActionUpdateCompleted("Two")
                .HasActionClearingCursor("Two")
                .HasActionUpdateCompleted("Parallel")
                .IsCheckedAll();

            /*
                Parallel = Completed
                    One = Completed
                    Two = Completed
            */

            CheckAllCompleted();
        }

        [TestMethod]
        public void BreakPointOnOneAndComplete()
        {
            SetBreakPoint("One")
                .HasActionSettingBreakPoint("One")
                .IsCheckedAll();

            /*
                Parallel = Waiting, Cursor
                    One = Waiting
                    Two = Waiting
            */

            Run("Parallel")
                .HasActionClearingCursor("Parallel")
                .HasActionUpdateRunning("Parallel")
                .HasActionSettingCursor("One")
                .HasActionUpdateStopped("One")
                .HasActionSettingCursor("Two")
                .HasActionUpdateToRun("Two")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = Stopped, Cursor
                    Two = ToRun, Cursor
            */

            SetRunning("Two")
                .HasActionUpdateRunning("Two")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = Stopped, Cursor
                    Two = Running, Cursor
            */

            SetCompleted("Two")
                .HasActionUpdateCompleted("Two")
                .HasActionClearingCursor("Two")
                .HasActionUpdateStopped("Parallel")
                .IsCheckedAll();

            /*
                Parallel = Stopped
                    One = Stopped, Cursor
                    Two = Completed
            */

            Run("One")
                .HasActionUpdateToRun("One")
                .HasActionUpdateRunning("Parallel")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = ToRun, Cursor
                    Two = Completed
            */

            SetRunning("One")
                .HasActionUpdateRunning("One")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = Running, Cursor
                    Two = Completed
            */

            SetCompleted("One")
                .HasActionUpdateCompleted("One")
                .HasActionClearingCursor("One")
                .HasActionUpdateCompleted("Parallel")
                .IsCheckedAll();

            /*
                Parallel = Completed
                    One = Completed
                    Two = Completed
            */

            CheckAllCompleted();
        }

        [TestMethod]
        public void BreakPointInBothThenErrorAndComplete()
        {
            SetBreakPoint("One")
                .HasActionSettingBreakPoint("One")
                .IsCheckedAll();

            SetBreakPoint("Two")
                .HasActionSettingBreakPoint("Two")
                .IsCheckedAll();

            /*
                Parallel = Waiting, Cursor
                    One = Waiting
                    Two = Waiting
            */

            Run("Parallel")
                .HasActionClearingCursor("Parallel")
                .HasActionUpdateStopped("Parallel")
                .HasActionSettingCursor("One")
                .HasActionUpdateStopped("One")
                .HasActionSettingCursor("Two")
                .HasActionUpdateStopped("Two")
                .IsCheckedAll();

            /*
                Parallel = Stopped
                    One = Stopped, Cursor
                    Two = Stopped, Cursor
            */

            Run("One")
                .HasActionUpdateToRun("One")
                .HasActionUpdateRunning("Parallel")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = ToRun, Cursor
                    Two = Stopped, Cursor
            */

            SetRunning("One")
                .HasActionUpdateRunning("One")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = Running, Cursor
                    Two = Stopped, Cursor
            */

            SetError("One")
                .HasActionUpdateError("One")
                .HasActionUpdateError("Parallel")
                .IsCheckedAll();

            /*
                Parallel = Error
                    One = Error, Cursor
                    Two = Stopped, Cursor
            */

            Run("Two")
                .HasActionUpdateToRun("Two")
                .HasActionUpdateRunning("Parallel")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = Error, Cursor
                    Two = ToRun, Cursor
            */


            SetRunning("Two")
                .HasActionUpdateRunning("Two")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = Error, Cursor
                    Two = Running, Cursor
            */

            SetCompleted("Two")
                .HasActionUpdateCompleted("Two")
                .HasActionClearingCursor("Two")
                .HasActionUpdateError("Parallel")
                .IsCheckedAll();

            /*
                Parallel = Error
                    One = Error, Cursor
                    Two = Completed
            */

            Run("One")
                .HasActionUpdateToRun("One")
                .HasActionUpdateRunning("Parallel")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = ToRun, Cursor
                    Two = Completed
            */

            SetRunning("One")
                .HasActionUpdateRunning("One")
                .IsCheckedAll();

            /*
                Parallel = Running
                    One = Running, Cursor
                    Two = Completed
            */

            SetCompleted("One")
                .HasActionUpdateCompleted("One")
                .HasActionClearingCursor("One")
                .HasActionUpdateCompleted("Parallel")
                .IsCheckedAll();

            /*
                Parallel = Completed
                    One = Completed
                    Two = Completed
            */

            CheckAllCompleted();
        }
    }
}
