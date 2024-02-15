using MongoDB.Bson;
using Runner.Business.Actions;
using Runner.Business.Entities.Nodes.Types;
using Runner.Business.Tests.Helpers;

namespace Runner.Business.Tests.Actions
{
    [TestClass]
    public class SingleContainerTest : TestActionsBase
    {
        protected override ActionControl GetControl()
        {
            var flow = new Flow
            {
                NodeId = ObjectId.Empty,
                Input = [],
                Root = new FlowAction
                {
                    Label = "Container",
                    Type = ActionType.Container,
                    Childs = new List<FlowAction>
                    {
                        new FlowAction
                        {
                            Label = "First",
                            Type = ActionType.Script
                        },
                        new FlowAction
                        {
                            Label = "Mid",
                            Type = ActionType.Script
                        },
                        new FlowAction
                        {
                            Label = "End",
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
                Container = Waiting, Cursor
                    First = Waiting
                    Mid = Waiting
                    End = Waiting
            */

            Run("Container")
                .HasActionClearingCursor("Container")
                .HasActionUpdateRunning("Container")
                .HasActionSettingCursor("First")
                .HasActionUpdateToRun("First")
                .IsCheckedAll();

            /*
                Container = Running
                    First = ToRun, Cursor
                    Mid = Waiting
                    End = Waiting
            */

            SetRunning("First")
                .HasActionUpdateRunning("First")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Running, Cursor
                    Mid = Waiting
                    End = Waiting
            */

            SetCompleted("First")
                .HasActionUpdateCompleted("First")
                .HasActionClearingCursor("First")
                .HasActionSettingCursor("Mid")
                .HasActionUpdateToRun("Mid")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = ToRun, Cursor
                    End = Waiting
            */

            SetRunning("Mid")
                .HasActionUpdateRunning("Mid")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Running, Cursor
                    End = Waiting
            */

            SetCompleted("Mid")
                .HasActionUpdateCompleted("Mid")
                .HasActionClearingCursor("Mid")
                .HasActionSettingCursor("End")
                .HasActionUpdateToRun("End")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Completed
                    End = ToRun, Cursor
            */

            SetRunning("End")
                .HasActionUpdateRunning("End")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Completed
                    End = Running, Cursor
            */

            SetCompleted("End")
                .HasActionUpdateCompleted("End")
                .HasActionClearingCursor("End")
                .HasActionUpdateCompleted("Container")
                .IsCheckedAll();

            /*
                Container = Completed
                    First = Completed
                    Mid = Completed
                    End = Completed
            */
        }

        [TestMethod]
        public void ErrorOnFirstRetryComplete()
        {
            /*
                Container = Waiting, Cursor
                    First = Waiting
                    Mid = Waiting
                    End = Waiting
            */

            Run("Container")
                .HasActionClearingCursor("Container")
                .HasActionUpdateRunning("Container")
                .HasActionSettingCursor("First")
                .HasActionUpdateToRun("First")
                .IsCheckedAll();

            /*
                Container = Running
                    First = ToRun, Cursor
                    Mid = Waiting
                    End = Waiting
            */

            SetRunning("First")
                .HasActionUpdateRunning("First")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Running, Cursor
                    Mid = Waiting
                    End = Waiting
            */

            SetError("First")
                .HasActionUpdateError("First")
                .HasActionUpdateError("Container")
                .IsCheckedAll();

            /*
                Container = Error
                    First = Error, Cursor
                    Mid = Waiting
                    End = Waiting
            */

            Run("First")
                .HasActionUpdateRunning("Container")
                .HasActionUpdateToRun("First")
                .IsCheckedAll();

            /*
                Container = Running
                    First = ToRun, Cursor
                    Mid = Waiting
                    End = Waiting
            */

            SetRunning("First")
                .HasActionUpdateRunning("First")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Running, Cursor
                    Mid = Waiting
                    End = Waiting
            */

            SetCompleted("First")
                .HasActionUpdateCompleted("First")
                .HasActionClearingCursor("First")
                .HasActionSettingCursor("Mid")
                .HasActionUpdateToRun("Mid")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = ToRun, Cursor
                    End = Waiting
            */

            SetRunning("Mid")
                .HasActionUpdateRunning("Mid")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Running, Cursor
                    End = Waiting
            */

            SetCompleted("Mid")
                .HasActionUpdateCompleted("Mid")
                .HasActionClearingCursor("Mid")
                .HasActionSettingCursor("End")
                .HasActionUpdateToRun("End")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Completed
                    End = ToRun, Cursor
            */

            SetRunning("End")
                .HasActionUpdateRunning("End")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Completed
                    End = Running, Cursor
            */

            SetCompleted("End")
                .HasActionUpdateCompleted("End")
                .HasActionClearingCursor("End")
                .HasActionUpdateCompleted("Container")
                .IsCheckedAll();

            /*
                Container = Completed
                    First = Completed
                    Mid = Completed
                    End = Completed
            */
        }

        [TestMethod]
        public void ErrorOnMidRetryComplete()
        {
            /*
                Container = Waiting, Cursor
                    First = Waiting
                    Mid = Waiting
                    End = Waiting
            */

            Run("Container")
                .HasActionClearingCursor("Container")
                .HasActionUpdateRunning("Container")
                .HasActionSettingCursor("First")
                .HasActionUpdateToRun("First")
                .IsCheckedAll();

            /*
                Container = Running
                    First = ToRun, Cursor
                    Mid = Waiting
                    End = Waiting
            */

            SetRunning("First")
                .HasActionUpdateRunning("First")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Running, Cursor
                    Mid = Waiting
                    End = Waiting
            */

            SetCompleted("First")
                .HasActionUpdateCompleted("First")
                .HasActionClearingCursor("First")
                .HasActionSettingCursor("Mid")
                .HasActionUpdateToRun("Mid")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = ToRun, Cursor
                    End = Waiting
            */

            SetRunning("Mid")
                .HasActionUpdateRunning("Mid")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Running, Cursor
                    End = Waiting
            */

            SetError("Mid")
                .HasActionUpdateError("Mid")
                .HasActionUpdateError("Container")
                .IsCheckedAll();

            /*
                Container = Error
                    First = Completed
                    Mid = Error, Cursor
                    End = Waiting
            */

            Run("Mid")
                .HasActionUpdateRunning("Container")
                .HasActionUpdateToRun("Mid")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = ToRun, Cursor
                    End = Waiting
            */

            SetRunning("Mid")
                .HasActionUpdateRunning("Mid")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Running, Cursor
                    End = Waiting
            */

            SetCompleted("Mid")
                .HasActionUpdateCompleted("Mid")
                .HasActionClearingCursor("Mid")
                .HasActionSettingCursor("End")
                .HasActionUpdateToRun("End")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Completed
                    End = ToRun, Cursor
            */

            SetRunning("End")
                .HasActionUpdateRunning("End")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Completed
                    End = Running, Cursor
            */

            SetCompleted("End")
                .HasActionUpdateCompleted("End")
                .HasActionClearingCursor("End")
                .HasActionUpdateCompleted("Container")
                .IsCheckedAll();

            /*
                Container = Completed
                    First = Completed
                    Mid = Completed
                    End = Completed
            */
        }

        [TestMethod]
        public void ErrorOnEndRetryComplete()
        {
            /*
                Container = Waiting, Cursor
                    First = Waiting
                    Mid = Waiting
                    End = Waiting
            */

            Run("Container")
                .HasActionClearingCursor("Container")
                .HasActionUpdateRunning("Container")
                .HasActionSettingCursor("First")
                .HasActionUpdateToRun("First")
                .IsCheckedAll();

            /*
                Container = Running
                    First = ToRun, Cursor
                    Mid = Waiting
                    End = Waiting
            */

            SetRunning("First")
                .HasActionUpdateRunning("First")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Running, Cursor
                    Mid = Waiting
                    End = Waiting
            */

            SetCompleted("First")
                .HasActionUpdateCompleted("First")
                .HasActionClearingCursor("First")
                .HasActionSettingCursor("Mid")
                .HasActionUpdateToRun("Mid")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = ToRun, Cursor
                    End = Waiting
            */

            SetRunning("Mid")
                .HasActionUpdateRunning("Mid")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Running, Cursor
                    End = Waiting
            */

            SetCompleted("Mid")
                .HasActionUpdateCompleted("Mid")
                .HasActionClearingCursor("Mid")
                .HasActionSettingCursor("End")
                .HasActionUpdateToRun("End")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Completed
                    End = ToRun, Cursor
            */

            SetRunning("End")
                .HasActionUpdateRunning("End")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Completed
                    End = Running, Cursor
            */

            SetError("End")
                .HasActionUpdateError("End")
                .HasActionUpdateError("Container")
                .IsCheckedAll();

            /*
                Container = Error
                    First = Completed
                    Mid = Completed
                    End = Error, Cursor
            */

            Run("End")
                .HasActionUpdateRunning("Container")
                .HasActionUpdateToRun("End")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Completed
                    End = ToRun, Cursor
            */

            SetRunning("End")
                .HasActionUpdateRunning("End")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Completed
                    End = Running, Cursor
            */

            SetCompleted("End")
                .HasActionUpdateCompleted("End")
                .HasActionClearingCursor("End")
                .HasActionUpdateCompleted("Container")
                .IsCheckedAll();

            /*
                Container = Completed
                    First = Completed
                    Mid = Completed
                    End = Completed
            */
        }

        [TestMethod]
        public void StopOnFirstAndContainue()
        {
            /*
                Container = Waiting, Cursor
                    First = Waiting
                    Mid = Waiting
                    End = Waiting
            */

            Run("Container")
                .HasActionClearingCursor("Container")
                .HasActionUpdateRunning("Container")
                .HasActionSettingCursor("First")
                .HasActionUpdateToRun("First")
                .IsCheckedAll();

            /*
                Container = Running
                    First = ToRun, Cursor
                    Mid = Waiting
                    End = Waiting
            */

            SetRunning("First")
                .HasActionUpdateRunning("First")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Running, Cursor
                    Mid = Waiting
                    End = Waiting
            */

            Stop("First")
                .HasActionUpdateToStop("First")
                .IsCheckedAll();

            /*
                Container = Running
                    First = ToStop, Cursor
                    Mid = Waiting
                    End = Waiting
            */

            SetStopped("First")
                .HasActionUpdateStopped("First")
                .HasActionUpdateStopped("Container")
                .IsCheckedAll();

            /*
                Container = Stopped
                    First = Stopped, Cursor
                    Mid = Waiting
                    End = Waiting
            */

            Run("First")
                .HasActionUpdateRunning("Container")
                .HasActionUpdateToRun("First")
                .IsCheckedAll();

            /*
                Container = Running
                    First = ToRun, Cursor
                    Mid = Waiting
                    End = Waiting
            */

            SetRunning("First")
                .HasActionUpdateRunning("First")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Running, Cursor
                    Mid = Waiting
                    End = Waiting
            */

            SetCompleted("First")
                .HasActionUpdateCompleted("First")
                .HasActionClearingCursor("First")
                .HasActionSettingCursor("Mid")
                .HasActionUpdateToRun("Mid")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = ToRun, Cursor
                    End = Waiting
            */

            SetRunning("Mid")
                .HasActionUpdateRunning("Mid")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Running, Cursor
                    End = Waiting
            */

            SetCompleted("Mid")
                .HasActionUpdateCompleted("Mid")
                .HasActionClearingCursor("Mid")
                .HasActionSettingCursor("End")
                .HasActionUpdateToRun("End")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Completed
                    End = ToRun, Cursor
            */

            SetRunning("End")
                .HasActionUpdateRunning("End")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Completed
                    End = Running, Cursor
            */

            SetCompleted("End")
                .HasActionUpdateCompleted("End")
                .HasActionClearingCursor("End")
                .HasActionUpdateCompleted("Container")
                .IsCheckedAll();

            /*
                Container = Completed
                    First = Completed
                    Mid = Completed
                    End = Completed
            */
        }

        [TestMethod]
        public void StopOnMidAndContainue()
        {
            /*
                Container = Waiting, Cursor
                    First = Waiting
                    Mid = Waiting
                    End = Waiting
            */

            Run("Container")
                .HasActionClearingCursor("Container")
                .HasActionUpdateRunning("Container")
                .HasActionSettingCursor("First")
                .HasActionUpdateToRun("First")
                .IsCheckedAll();

            /*
                Container = Running
                    First = ToRun, Cursor
                    Mid = Waiting
                    End = Waiting
            */

            SetRunning("First")
                .HasActionUpdateRunning("First")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Running, Cursor
                    Mid = Waiting
                    End = Waiting
            */

            SetCompleted("First")
                .HasActionUpdateCompleted("First")
                .HasActionClearingCursor("First")
                .HasActionSettingCursor("Mid")
                .HasActionUpdateToRun("Mid")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = ToRun, Cursor
                    End = Waiting
            */

            SetRunning("Mid")
                .HasActionUpdateRunning("Mid")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Running, Cursor
                    End = Waiting
            */

            Stop("Mid")
                .HasActionUpdateToStop("Mid")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = ToStop, Cursor
                    End = Waiting
            */

            SetStopped("Mid")
                .HasActionUpdateStopped("Mid")
                .HasActionUpdateStopped("Container")
                .IsCheckedAll();

            /*
                Container = Stopped
                    First = Completed
                    Mid = Stopped, Cursor
                    End = Waiting
            */

            Run("Mid")
                .HasActionUpdateRunning("Container")
                .HasActionUpdateToRun("Mid")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = ToRun, Cursor
                    End = Waiting
            */

            SetRunning("Mid")
                .HasActionUpdateRunning("Mid")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Running, Cursor
                    End = Waiting
            */

            SetCompleted("Mid")
                .HasActionUpdateCompleted("Mid")
                .HasActionClearingCursor("Mid")
                .HasActionSettingCursor("End")
                .HasActionUpdateToRun("End")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Completed
                    End = ToRun, Cursor
            */

            SetRunning("End")
                .HasActionUpdateRunning("End")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Completed
                    End = Running, Cursor
            */

            SetCompleted("End")
                .HasActionUpdateCompleted("End")
                .HasActionClearingCursor("End")
                .HasActionUpdateCompleted("Container")
                .IsCheckedAll();

            /*
                Container = Completed
                    First = Completed
                    Mid = Completed
                    End = Completed
            */
        }

        [TestMethod]
        public void StopOnEndAndContainue()
        {
            /*
                Container = Waiting, Cursor
                    First = Waiting
                    Mid = Waiting
                    End = Waiting
            */

            Run("Container")
                .HasActionClearingCursor("Container")
                .HasActionUpdateRunning("Container")
                .HasActionSettingCursor("First")
                .HasActionUpdateToRun("First")
                .IsCheckedAll();

            /*
                Container = Running
                    First = ToRun, Cursor
                    Mid = Waiting
                    End = Waiting
            */

            SetRunning("First")
                .HasActionUpdateRunning("First")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Running, Cursor
                    Mid = Waiting
                    End = Waiting
            */

            SetCompleted("First")
                .HasActionUpdateCompleted("First")
                .HasActionClearingCursor("First")
                .HasActionSettingCursor("Mid")
                .HasActionUpdateToRun("Mid")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = ToRun, Cursor
                    End = Waiting
            */

            SetRunning("Mid")
                .HasActionUpdateRunning("Mid")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Running, Cursor
                    End = Waiting
            */

            SetCompleted("Mid")
                .HasActionUpdateCompleted("Mid")
                .HasActionClearingCursor("Mid")
                .HasActionSettingCursor("End")
                .HasActionUpdateToRun("End")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Completed
                    End = ToRun, Cursor
            */

            SetRunning("End")
                .HasActionUpdateRunning("End")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Completed
                    End = Running, Cursor
            */

            Stop("End")
                .HasActionUpdateToStop("End")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Completed
                    End = ToStop, Cursor
            */

            SetStopped("End")
                .HasActionUpdateStopped("End")
                .HasActionUpdateStopped("Container")
                .IsCheckedAll();

            /*
                Container = Stopped
                    First = Completed
                    Mid = Completed
                    End = Stopped, Cursor
            */

            Run("End")
                .HasActionUpdateRunning("Container")
                .HasActionUpdateToRun("End")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Completed
                    End = ToRun, Cursor
            */

            SetRunning("End")
                .HasActionUpdateRunning("End")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Completed
                    End = Running, Cursor
            */

            SetCompleted("End")
                .HasActionUpdateCompleted("End")
                .HasActionClearingCursor("End")
                .HasActionUpdateCompleted("Container")
                .IsCheckedAll();

            /*
                Container = Completed
                    First = Completed
                    Mid = Completed
                    End = Completed
            */
        }

        [TestMethod]
        public void BreakPointOnFirstAndContainue()
        {
            SetBreakPoint("First")
                .HasActionSettingBreakPoint("First")
                .IsCheckedAll();

            /*
                Container = Waiting, Cursor
                    First = Waiting
                    Mid = Waiting
                    End = Waiting
            */

            Run("Container")
                .HasActionClearingCursor("Container")
                .HasActionSettingCursor("First")
                .HasActionUpdateStopped("First")
                .HasActionUpdateStopped("Container")
                .IsCheckedAll();

            /*
                Container = Stopped
                    First = Stopped, Cursor
                    Mid = Waiting
                    End = Waiting
            */

            Run("First")
                .HasActionUpdateRunning("Container")
                .HasActionUpdateToRun("First")
                .IsCheckedAll();

            /*
                Container = Running
                    First = ToRun, Cursor
                    Mid = Waiting
                    End = Waiting
            */

            SetRunning("First")
                .HasActionUpdateRunning("First")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Running, Cursor
                    Mid = Waiting
                    End = Waiting
            */

            SetCompleted("First")
                .HasActionUpdateCompleted("First")
                .HasActionClearingCursor("First")
                .HasActionSettingCursor("Mid")
                .HasActionUpdateToRun("Mid")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = ToRun, Cursor
                    End = Waiting
            */

            SetRunning("Mid")
                .HasActionUpdateRunning("Mid")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Running, Cursor
                    End = Waiting
            */

            SetCompleted("Mid")
                .HasActionUpdateCompleted("Mid")
                .HasActionClearingCursor("Mid")
                .HasActionSettingCursor("End")
                .HasActionUpdateToRun("End")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Completed
                    End = ToRun, Cursor
            */

            SetRunning("End")
                .HasActionUpdateRunning("End")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Completed
                    End = Running, Cursor
            */

            SetCompleted("End")
                .HasActionUpdateCompleted("End")
                .HasActionClearingCursor("End")
                .HasActionUpdateCompleted("Container")
                .IsCheckedAll();

            /*
                Container = Completed
                    First = Completed
                    Mid = Completed
                    End = Completed
            */
        }

        [TestMethod]
        public void BreakPointOnMidAndContainue()
        {
            SetBreakPoint("Mid")
                .HasActionSettingBreakPoint("Mid")
                .IsCheckedAll();

            /*
                Container = Waiting, Cursor
                    First = Waiting
                    Mid = Waiting
                    End = Waiting
            */

            Run("Container")
                .HasActionClearingCursor("Container")
                .HasActionUpdateRunning("Container")
                .HasActionSettingCursor("First")
                .HasActionUpdateToRun("First")
                .IsCheckedAll();

            /*
                Container = Running
                    First = ToRun, Cursor
                    Mid = Waiting
                    End = Waiting
            */

            SetRunning("First")
                .HasActionUpdateRunning("First")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Running, Cursor
                    Mid = Waiting
                    End = Waiting
            */

            SetCompleted("First")
                .HasActionUpdateCompleted("First")
                .HasActionClearingCursor("First")
                .HasActionSettingCursor("Mid")
                .HasActionUpdateStopped("Mid")
                .HasActionUpdateStopped("Container")
                .IsCheckedAll();

            /*
                Container = Stopped
                    First = Completed
                    Mid = Stopped, Cursor
                    End = Waiting
            */

            Run("Mid")
                .HasActionUpdateRunning("Container")
                .HasActionUpdateToRun("Mid")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = ToRun, Cursor
                    End = Waiting
            */

            SetRunning("Mid")
                .HasActionUpdateRunning("Mid")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Running, Cursor
                    End = Waiting
            */

            SetCompleted("Mid")
                .HasActionUpdateCompleted("Mid")
                .HasActionClearingCursor("Mid")
                .HasActionSettingCursor("End")
                .HasActionUpdateToRun("End")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Completed
                    End = ToRun, Cursor
            */

            SetRunning("End")
                .HasActionUpdateRunning("End")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Completed
                    End = Running, Cursor
            */

            SetCompleted("End")
                .HasActionUpdateCompleted("End")
                .HasActionClearingCursor("End")
                .HasActionUpdateCompleted("Container")
                .IsCheckedAll();

            /*
                Container = Completed
                    First = Completed
                    Mid = Completed
                    End = Completed
            */
        }

        [TestMethod]
        public void BreakPointOnEndAndContainue()
        {
            SetBreakPoint("End")
               .HasActionSettingBreakPoint("End")
               .IsCheckedAll();

            /*
                Container = Waiting, Cursor
                    First = Waiting
                    Mid = Waiting
                    End = Waiting
            */

            Run("Container")
                .HasActionClearingCursor("Container")
                .HasActionUpdateRunning("Container")
                .HasActionSettingCursor("First")
                .HasActionUpdateToRun("First")
                .IsCheckedAll();

            /*
                Container = Running
                    First = ToRun, Cursor
                    Mid = Waiting
                    End = Waiting
            */

            SetRunning("First")
                .HasActionUpdateRunning("First")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Running, Cursor
                    Mid = Waiting
                    End = Waiting
            */

            SetCompleted("First")
                .HasActionUpdateCompleted("First")
                .HasActionClearingCursor("First")
                .HasActionSettingCursor("Mid")
                .HasActionUpdateToRun("Mid")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = ToRun, Cursor
                    End = Waiting
            */

            SetRunning("Mid")
                .HasActionUpdateRunning("Mid")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Running, Cursor
                    End = Waiting
            */

            SetCompleted("Mid")
                .HasActionUpdateCompleted("Mid")
                .HasActionClearingCursor("Mid")
                .HasActionSettingCursor("End")
                .HasActionUpdateStopped("End")
                .HasActionUpdateStopped("Container")
                .IsCheckedAll();

            /*
                Container = Stopped
                    First = Completed
                    Mid = Completed
                    End = Stopped, Cursor
            */

            Run("End")
                .HasActionUpdateRunning("Container")
                .HasActionUpdateToRun("End")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Completed
                    End = ToRun, Cursor
            */

            SetRunning("End")
                .HasActionUpdateRunning("End")
                .IsCheckedAll();

            /*
                Container = Running
                    First = Completed
                    Mid = Completed
                    End = Running, Cursor
            */

            SetCompleted("End")
                .HasActionUpdateCompleted("End")
                .HasActionClearingCursor("End")
                .HasActionUpdateCompleted("Container")
                .IsCheckedAll();

            /*
                Container = Completed
                    First = Completed
                    Mid = Completed
                    End = Completed
            */
        }
    }
}
