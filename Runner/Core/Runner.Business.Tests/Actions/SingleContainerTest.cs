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
    public class SingleContainerTest : TestActionsBase2
    {
        protected override ActionControl GetControl()
        {
            var flow = new Flow2
            {
                Name = "Test",
                Root = new FlowAction2
                {
                    Label = "Container",
                    Type = ActionType.Container,
                    Childs = new List<FlowAction2>
                    {
                        new FlowAction2
                        {
                            Label = "First",
                            Type = ActionType.Script
                        },
                        new FlowAction2
                        {
                            Label = "Mid",
                            Type = ActionType.Script
                        },
                        new FlowAction2
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
            Run("Container")
                .TestCount(3)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateStatus(ActionStatus.Running)
                .TestInSequeceActionUpdateToRun();

            SetRunning("First")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            SetCompleted("First")
                .TestCount(3)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateToRun();

            SetRunning("Mid")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            SetCompleted("Mid")
                .TestCount(3)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateToRun();

            SetRunning("End")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            SetCompleted("End")
                .TestCount(2)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed);
        }

        [TestMethod]
        public void ErrorOnFirstRetryComplete()
        {
            Run("Container")
                .TestCount(3)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateStatus(ActionStatus.Running)
                .TestInSequeceActionUpdateToRun();

            SetRunning("First")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            SetError("First")
                .TestCount(2)
                .TestInSequeceActionUpdateStatus(ActionStatus.Error)
                .TestInSequeceActionUpdateStatus(ActionStatus.Error);

            Run("First")
                .TestCount(2)
                .TestInSequeceActionUpdateToRun()
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);
            
            SetRunning("First")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            SetCompleted("First")
                .TestCount(3)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateToRun();

            SetRunning("Mid")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            SetCompleted("Mid")
                .TestCount(3)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateToRun();

            SetRunning("End")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            SetCompleted("End")
                .TestCount(2)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed);
        }

        [TestMethod]
        public void ErrorOnMidRetryComplete()
        {
            // start run the flow
            Run("Container")
                .TestCount(3)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateStatus(ActionStatus.Running)
                .TestInSequeceActionUpdateToRun();

            // job begin to run the first script
            SetRunning("First")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job complete first script
            SetCompleted("First")
                .TestCount(3)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateToRun();

            // job begin to run mid script
            SetRunning("Mid")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job raiser error on mid script
            // set container as error
            SetError("Mid")
                .TestCount(2)
                .TestInSequeceActionUpdateStatus(ActionStatus.Error)
                .TestInSequeceActionUpdateStatus(ActionStatus.Error);

            // user send to retry mid script
            // set container as running
            Run("Mid")
                .TestCount(2)
                .TestInSequeceActionUpdateToRun()
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job begin to retry mid script
            SetRunning("Mid")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job complete mid script
            SetCompleted("Mid")
                .TestCount(3)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateToRun();

            // job begin to run end script
            SetRunning("End")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job complete end script
            // set container as completed
            SetCompleted("End")
                .TestCount(2)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed);
        }

        [TestMethod]
        public void ErrorOnEndRetryComplete()
        {
            // start run the flow
            // set container to running
            // set to run first script
            Run("Container")
                .TestCount(3)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateStatus(ActionStatus.Running)
                .TestInSequeceActionUpdateToRun();

            // job begin to run the first script
            SetRunning("First")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job complete first script
            // set to run mid script
            SetCompleted("First")
                .TestCount(3)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateToRun();

            // job begin to run mid script
            SetRunning("Mid")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job complete mid script
            // set to run end script
            SetCompleted("Mid")
                .TestCount(3)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateToRun();

            // job begin to run end script
            SetRunning("End")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job raiser error on end script
            // set container as error
            SetError("End")
                .TestCount(2)
                .TestInSequeceActionUpdateStatus(ActionStatus.Error)
                .TestInSequeceActionUpdateStatus(ActionStatus.Error);

            // user send to retry end script
            // set container as running
            Run("End")
                .TestCount(2)
                .TestInSequeceActionUpdateToRun()
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job begin to retry end script
            SetRunning("End")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job complete end script
            // set container as completed
            SetCompleted("End")
                .TestCount(2)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed);
        }

        [TestMethod]
        public void StopOnFirstAndContainue()
        {
            // start run the flow
            // set container to running
            // set to run first script
            Run("Container")
                .TestCount(3)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateStatus(ActionStatus.Running)
                .TestInSequeceActionUpdateToRun();
            
            // job begin to run the first script
            SetRunning("First")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // user set to stop first script
            Stop("First")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.ToStop);

            // job signal that first script is stopped
            SetStopped("First")
                .TestCount(2)
                .TestInSequeceActionUpdateStatus(ActionStatus.Stopped)
                .TestInSequeceActionUpdateStatus(ActionStatus.Stopped);
            
            // user send to retry first script
            // set container as running
            Run("First")
                .TestCount(2)
                .TestInSequeceActionUpdateToRun()
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job begin to run the first script
            SetRunning("First")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job complete first script
            // set to run mid script
            SetCompleted("First")
                .TestCount(3)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateToRun();

            // job begin to run mid script
            SetRunning("Mid")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job complete mid script
            // set to run end script
            SetCompleted("Mid")
                .TestCount(3)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateToRun();

            // job begin to run end script
            SetRunning("End")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job complete end script
            // set container as completed
            SetCompleted("End")
                .TestCount(2)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed);
        }

        [TestMethod]
        public void StopOnMidAndContainue()
        {
            // start run the flow
            // set container to running
            // set to run first script
            Run("Container")
                .TestCount(3)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateStatus(ActionStatus.Running)
                .TestInSequeceActionUpdateToRun();

            // job begin to run the first script
            SetRunning("First")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job complete first script
            // set to run mid script
            SetCompleted("First")
                .TestCount(3)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateToRun();

            // job begin to run mid script
            SetRunning("Mid")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // user set to stop mid script
            Stop("Mid")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.ToStop);

            // job signal that mid script is stopped
            SetStopped("Mid")
                .TestCount(2)
                .TestInSequeceActionUpdateStatus(ActionStatus.Stopped)
                .TestInSequeceActionUpdateStatus(ActionStatus.Stopped);

            // user send to retry mid script
            // set container as running
            Run("Mid")
                .TestCount(2)
                .TestInSequeceActionUpdateToRun()
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job begin to run the mid script
            SetRunning("Mid")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job complete mid script
            // set to run end script
            SetCompleted("Mid")
                .TestCount(3)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateToRun();

            // job begin to run end script
            SetRunning("End")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job complete end script
            // set container as completed
            SetCompleted("End")
                .TestCount(2)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed);
        }

        [TestMethod]
        public void StopOnEndAndContainue()
        {
            // start run the flow
            // set container to running
            // set to run first script
            Run("Container")
                .TestCount(3)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateStatus(ActionStatus.Running)
                .TestInSequeceActionUpdateToRun();

            // job begin to run the first script
            SetRunning("First")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job complete first script
            // set to run mid script
            SetCompleted("First")
                .TestCount(3)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateToRun();

            // job begin to run mid script
            SetRunning("Mid")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job complete mid script
            // set to run end script
            SetCompleted("Mid")
                .TestCount(3)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateToRun();

            // job begin to run end script
            SetRunning("End")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // user set to stop end script
            Stop("End")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.ToStop);

            // job signal that end script is stopped
            SetStopped("End")
                .TestCount(2)
                .TestInSequeceActionUpdateStatus(ActionStatus.Stopped)
                .TestInSequeceActionUpdateStatus(ActionStatus.Stopped);

            // user send to retry end script
            // set container as running
            Run("End")
                .TestCount(2)
                .TestInSequeceActionUpdateToRun()
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job begin to run the end script
            SetRunning("End")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job complete end script
            // set container as completed
            SetCompleted("End")
                .TestCount(2)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed);
        }

        [TestMethod]
        public void StopContainerOnFirstAndContainue()
        {
            // start run the flow
            // set container to running
            // set to run first script
            Run("Container")
                .TestCount(3)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateStatus(ActionStatus.Running)
                .TestInSequeceActionUpdateToRun();

            // job begin to run the first script
            SetRunning("First")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // user set to stop on container
            Stop("Container")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.ToStop);

            // job signal that first script is stopped
            SetStopped("First")
                .TestCount(2)
                .TestInSequeceActionUpdateStatus(ActionStatus.Stopped)
                .TestInSequeceActionUpdateStatus(ActionStatus.Stopped);

            // user send to retry first script
            // set container as running
            Run("First")
                .TestCount(2)
                .TestInSequeceActionUpdateToRun()
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job begin to run the first script
            SetRunning("First")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job complete first script
            // set to run mid script
            SetCompleted("First")
                .TestCount(3)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateToRun();

            // job begin to run mid script
            SetRunning("Mid")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job complete mid script
            // set to run end script
            SetCompleted("Mid")
                .TestCount(3)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateToRun();

            // job begin to run end script
            SetRunning("End")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job complete end script
            // set container as completed
            SetCompleted("End")
                .TestCount(2)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed);
        }

        [TestMethod]
        public void StopContainerOnMidAndContainue()
        {
            // start run the flow
            // set container to running
            // set to run first script
            Run("Container")
                .TestCount(3)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateStatus(ActionStatus.Running)
                .TestInSequeceActionUpdateToRun();

            // job begin to run the first script
            SetRunning("First")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job complete first script
            // set to run mid script
            SetCompleted("First")
                .TestCount(3)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateToRun();

            // job begin to run mid script
            SetRunning("Mid")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // user set to stop mid script
            Stop("Container")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.ToStop);

            // job signal that mid script is stopped
            SetStopped("Mid")
                .TestCount(2)
                .TestInSequeceActionUpdateStatus(ActionStatus.Stopped)
                .TestInSequeceActionUpdateStatus(ActionStatus.Stopped);

            // user send to retry mid script
            // set container as running
            Run("Mid")
                .TestCount(2)
                .TestInSequeceActionUpdateToRun()
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job begin to run the mid script
            SetRunning("Mid")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job complete mid script
            // set to run end script
            SetCompleted("Mid")
                .TestCount(3)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateToRun();

            // job begin to run end script
            SetRunning("End")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job complete end script
            // set container as completed
            SetCompleted("End")
                .TestCount(2)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed);
        }

        [TestMethod]
        public void StopContainerOnEndAndContainue()
        {
            // start run the flow
            // set container to running
            // set to run first script
            Run("Container")
                .TestCount(3)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateStatus(ActionStatus.Running)
                .TestInSequeceActionUpdateToRun();

            // job begin to run the first script
            SetRunning("First")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job complete first script
            // set to run mid script
            SetCompleted("First")
                .TestCount(3)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateToRun();

            // job begin to run mid script
            SetRunning("Mid")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job complete mid script
            // set to run end script
            SetCompleted("Mid")
                .TestCount(3)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateToRun();

            // job begin to run end script
            SetRunning("End")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // user set to stop end script
            Stop("Container")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.ToStop);

            // job signal that end script is stopped
            SetStopped("End")
                .TestCount(2)
                .TestInSequeceActionUpdateStatus(ActionStatus.Stopped)
                .TestInSequeceActionUpdateStatus(ActionStatus.Stopped);

            // user send to retry end script
            // set container as running
            Run("End")
                .TestCount(2)
                .TestInSequeceActionUpdateToRun()
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job begin to run the end script
            SetRunning("End")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job complete end script
            // set container as completed
            SetCompleted("End")
                .TestCount(2)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed);
        }

        [TestMethod]
        public void BreakPointOnFirstAndContainue()
        {
            SetBreakPoint("First")
                .TestCount(1)
                .TestInSequeceActionUpdateBreakPoint(true);

            // start run the flow
            // container move the cursor, but stop on first script
            // set the container as stopped
            Run("Container")
                .TestCount(2)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateStatus(ActionStatus.Stopped);

            CleanBreakPoint("First")
                .TestCount(1)
                .TestInSequeceActionUpdateBreakPoint(false);

            // start run the flow
            // set container to running
            // set to run first script
            Run("Container")
                .TestCount(2)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running)
                .TestInSequeceActionUpdateToRun();

            // job begin to run the first script
            SetRunning("First")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job complete first script
            // set to run mid script
            SetCompleted("First")
                .TestCount(3)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateToRun();

            // job begin to run mid script
            SetRunning("Mid")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job complete mid script
            // set to run end script
            SetCompleted("Mid")
                .TestCount(3)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateToRun();

            // job begin to run end script
            SetRunning("End")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job complete end script
            // set container as completed
            SetCompleted("End")
                .TestCount(2)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed);
        }

        [TestMethod]
        public void BreakPointOnMidAndContainue()
        {
            SetBreakPoint("Mid")
                .TestCount(1)
                .TestInSequeceActionUpdateBreakPoint(true);

            // start run the flow
            // set container to running
            // set to run first script
            Run("Container")
                .TestCount(3)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateStatus(ActionStatus.Running)
                .TestInSequeceActionUpdateToRun();

            // job begin to run the first script
            SetRunning("First")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job complete first script
            // just move the cursor to mid and break
            // set the container as stopped
            SetCompleted("First")
                .TestCount(3)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateStatus(ActionStatus.Stopped);

            CleanBreakPoint("Mid")
                .TestCount(1)
                .TestInSequeceActionUpdateBreakPoint(false);

            // start run the flow
            // set container to running
            // set to run mid script
            Run("Container")
                .TestCount(2)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running)
                .TestInSequeceActionUpdateToRun();

            // job begin to run mid script
            SetRunning("Mid")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job complete mid script
            // set to run end script
            SetCompleted("Mid")
                .TestCount(3)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateToRun();

            // job begin to run end script
            SetRunning("End")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job complete end script
            // set container as completed
            SetCompleted("End")
                .TestCount(2)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed);
        }

        [TestMethod]
        public void BreakPointOnEndAndContainue()
        {
            SetBreakPoint("End")
                .TestCount(1)
                .TestInSequeceActionUpdateBreakPoint(true);

            // start run the flow
            // set container to running
            // set to run first script
            Run("Container")
                .TestCount(3)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateStatus(ActionStatus.Running)
                .TestInSequeceActionUpdateToRun();

            // job begin to run the first script
            SetRunning("First")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job complete first script
            // just move the cursor to mid
            // set to run mid script
            SetCompleted("First")
                .TestCount(3)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateToRun();

            // job begin to run mid script
            SetRunning("Mid")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job complete mid script
            // just move the cursor to end and break
            // set the container as stopped
            SetCompleted("Mid")
                .TestCount(3)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceCursorUpdate()
                .TestInSequeceActionUpdateStatus(ActionStatus.Stopped);

            CleanBreakPoint("End")
                .TestCount(1)
                .TestInSequeceActionUpdateBreakPoint(false);

            // start run the flow
            // set to run end script
            // set container to running
            Run("End")
                .TestCount(2)
                .TestInSequeceActionUpdateToRun()
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job begin to run end script
            SetRunning("End")
                .TestCount(1)
                .TestInSequeceActionUpdateStatus(ActionStatus.Running);

            // job complete end script
            // set container as completed
            SetCompleted("End")
                .TestCount(2)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed)
                .TestInSequeceActionUpdateStatus(ActionStatus.Completed);
        }
    }
}
