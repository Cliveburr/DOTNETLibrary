using Runner.Business.Actions;
using Runner.Business.Entities.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Tests.Actions
{
    [TestClass]
    public class ContainerWithContainerTest : Helpers.TestActionsBase
    {
        protected override ActionControl GetControl()
        {
            var flow = new Flow
            {
                Name = "Test",
                Root = new FlowAction
                {
                    Label = "Root",
                    Type = ActionType.Container,
                    Childs = new List<FlowAction>
                    {
                        new FlowAction
                        {
                            Label = "ContainerFirst",
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
                                    Label = "Second",
                                    Type = ActionType.Script
                                }
                            }
                        },
                        new FlowAction
                        {
                            Label = "Third",
                            Type = ActionType.Script
                        },
                        new FlowAction
                        {
                            Label = "ContainerSecond",
                            Type = ActionType.Container,
                            Childs = new List<FlowAction>
                            {
                                new FlowAction
                                {
                                    Label = "Four",
                                    Type = ActionType.Script
                                }
                            }
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
                Root = Waiting, Cursor
                    ContainerFirst = Waiting
                        First = Waiting
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            Run("Root")
                .HasActionClearingCursor("Root")
                .HasActionUpdateRunning("Root")
                .HasActionUpdateRunning("ContainerFirst")
                .HasActionSettingCursor("First")
                .HasActionUpdateToRun("First")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = ToRun, Cursor
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("First")
                .HasActionUpdateRunning("First")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Running, Cursor
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("First")
                .HasActionUpdateCompleted("First")
                .HasActionClearingCursor("First")
                .HasActionSettingCursor("Second")
                .HasActionUpdateToRun("Second")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Completed
                        Second = ToRun, Cursor
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("Second")
                .HasActionUpdateRunning("Second")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Completed
                        Second = Running, Cursor
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("Second")
                .HasActionUpdateCompleted("Second")
                .HasActionClearingCursor("Second")
                .HasActionUpdateCompleted("ContainerFirst")
                .HasActionSettingCursor("Third")
                .HasActionUpdateToRun("Third")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = ToRun, Cursor
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("Third")
                .HasActionUpdateRunning("Third")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Running, Cursor
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("Third")
                .HasActionUpdateCompleted("Third")
                .HasActionClearingCursor("Third")
                .HasActionUpdateRunning("ContainerSecond")
                .HasActionSettingCursor("Four")
                .HasActionUpdateToRun("Four")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Running
                        Four = ToRun, Cursor
            */

            SetRunning("Four")
                .HasActionUpdateRunning("Four")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Running
                        Four = Running, Cursor
            */

            SetCompleted("Four")
                .HasActionUpdateCompleted("Four")
                .HasActionClearingCursor("Four")
                .HasActionUpdateCompleted("ContainerSecond")
                .HasActionUpdateCompleted("Root")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Completed
                        Four = Completed
            */

            CheckAllCompleted();
        }

        [TestMethod]
        public void ErrorOnFirstRetryComplete()
        {
            /*
                Root = Waiting, Cursor
                    ContainerFirst = Waiting
                        First = Waiting
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            Run("Root")
                .HasActionClearingCursor("Root")
                .HasActionUpdateRunning("Root")
                .HasActionUpdateRunning("ContainerFirst")
                .HasActionSettingCursor("First")
                .HasActionUpdateToRun("First")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = ToRun, Cursor
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("First")
                .HasActionUpdateRunning("First")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Running, Cursor
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetError("First")
                .HasActionUpdateError("First")
                .HasActionUpdateError("ContainerFirst")
                .HasActionUpdateError("Root")
                .IsCheckedAll();

            /*
                Root = Error
                    ContainerFirst = Error
                        First = Error, Cursor
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            Run("First")
                .HasActionUpdateRunning("ContainerFirst")
                .HasActionUpdateRunning("Root")
                .HasActionUpdateToRun("First")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = ToRun, Cursor
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("First")
                .HasActionUpdateRunning("First")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Running, Cursor
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("First")
                .HasActionUpdateCompleted("First")
                .HasActionClearingCursor("First")
                .HasActionSettingCursor("Second")
                .HasActionUpdateToRun("Second")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Completed
                        Second = ToRun, Cursor
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("Second")
                .HasActionUpdateRunning("Second")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Completed
                        Second = Running, Cursor
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("Second")
                .HasActionUpdateCompleted("Second")
                .HasActionClearingCursor("Second")
                .HasActionUpdateCompleted("ContainerFirst")
                .HasActionSettingCursor("Third")
                .HasActionUpdateToRun("Third")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = ToRun, Cursor
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("Third")
                .HasActionUpdateRunning("Third")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Running, Cursor
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("Third")
                .HasActionUpdateCompleted("Third")
                .HasActionClearingCursor("Third")
                .HasActionUpdateRunning("ContainerSecond")
                .HasActionSettingCursor("Four")
                .HasActionUpdateToRun("Four")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Running
                        Four = ToRun, Cursor
            */

            SetRunning("Four")
                .HasActionUpdateRunning("Four")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Running
                        Four = Running, Cursor
            */

            SetCompleted("Four")
                .HasActionUpdateCompleted("Four")
                .HasActionClearingCursor("Four")
                .HasActionUpdateCompleted("ContainerSecond")
                .HasActionUpdateCompleted("Root")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Completed
                        Four = Completed
            */

            CheckAllCompleted();
        }

        [TestMethod]
        public void ErrorOnSecondRetryComplete()
        {
            /*
                Root = Waiting, Cursor
                    ContainerFirst = Waiting
                        First = Waiting
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            Run("Root")
                .HasActionClearingCursor("Root")
                .HasActionUpdateRunning("Root")
                .HasActionUpdateRunning("ContainerFirst")
                .HasActionSettingCursor("First")
                .HasActionUpdateToRun("First")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = ToRun, Cursor
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("First")
                .HasActionUpdateRunning("First")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Running, Cursor
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("First")
                .HasActionUpdateCompleted("First")
                .HasActionClearingCursor("First")
                .HasActionSettingCursor("Second")
                .HasActionUpdateToRun("Second")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Completed
                        Second = ToRun, Cursor
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("Second")
                .HasActionUpdateRunning("Second")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Completed
                        Second = Running, Cursor
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */


            SetError("Second")
                .HasActionUpdateError("Second")
                .HasActionUpdateError("ContainerFirst")
                .HasActionUpdateError("Root")
                .IsCheckedAll();

            /*
                Root = Error
                    ContainerFirst = Error
                        First = Completed
                        Second = Error, Cursor
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            Run("Second")
                .HasActionUpdateRunning("ContainerFirst")
                .HasActionUpdateRunning("Root")
                .HasActionUpdateToRun("Second")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Completed
                        Second = ToRun, Cursor
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("Second")
                .HasActionUpdateRunning("Second")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Completed
                        Second = Running, Cursor
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("Second")
                .HasActionUpdateCompleted("Second")
                .HasActionClearingCursor("Second")
                .HasActionUpdateCompleted("ContainerFirst")
                .HasActionSettingCursor("Third")
                .HasActionUpdateToRun("Third")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = ToRun, Cursor
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("Third")
                .HasActionUpdateRunning("Third")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Running, Cursor
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("Third")
                .HasActionUpdateCompleted("Third")
                .HasActionClearingCursor("Third")
                .HasActionUpdateRunning("ContainerSecond")
                .HasActionSettingCursor("Four")
                .HasActionUpdateToRun("Four")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Running
                        Four = ToRun, Cursor
            */

            SetRunning("Four")
                .HasActionUpdateRunning("Four")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Running
                        Four = Running, Cursor
            */

            SetCompleted("Four")
                .HasActionUpdateCompleted("Four")
                .HasActionClearingCursor("Four")
                .HasActionUpdateCompleted("ContainerSecond")
                .HasActionUpdateCompleted("Root")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Completed
                        Four = Completed
            */

            CheckAllCompleted();
        }

        [TestMethod]
        public void ErrorOnThirdRetryComplete()
        {
            /*
                Root = Waiting, Cursor
                    ContainerFirst = Waiting
                        First = Waiting
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            Run("Root")
                .HasActionClearingCursor("Root")
                .HasActionUpdateRunning("Root")
                .HasActionUpdateRunning("ContainerFirst")
                .HasActionSettingCursor("First")
                .HasActionUpdateToRun("First")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = ToRun, Cursor
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("First")
                .HasActionUpdateRunning("First")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Running, Cursor
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("First")
                .HasActionUpdateCompleted("First")
                .HasActionClearingCursor("First")
                .HasActionSettingCursor("Second")
                .HasActionUpdateToRun("Second")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Completed
                        Second = ToRun, Cursor
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("Second")
                .HasActionUpdateRunning("Second")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Completed
                        Second = Running, Cursor
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("Second")
                .HasActionUpdateCompleted("Second")
                .HasActionClearingCursor("Second")
                .HasActionUpdateCompleted("ContainerFirst")
                .HasActionSettingCursor("Third")
                .HasActionUpdateToRun("Third")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = ToRun, Cursor
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("Third")
                .HasActionUpdateRunning("Third")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Running, Cursor
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetError("Third")
                .HasActionUpdateError("Third")
                .HasActionUpdateError("Root")
                .IsCheckedAll();

            /*
                Root = Error
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Error, Cursor
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            Run("Third")
                .HasActionUpdateRunning("Root")
                .HasActionUpdateToRun("Third")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = ToRun, Cursor
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("Third")
                .HasActionUpdateRunning("Third")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Running, Cursor
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("Third")
                .HasActionUpdateCompleted("Third")
                .HasActionClearingCursor("Third")
                .HasActionUpdateRunning("ContainerSecond")
                .HasActionSettingCursor("Four")
                .HasActionUpdateToRun("Four")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Running
                        Four = ToRun, Cursor
            */

            SetRunning("Four")
                .HasActionUpdateRunning("Four")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Running
                        Four = Running, Cursor
            */

            SetCompleted("Four")
                .HasActionUpdateCompleted("Four")
                .HasActionClearingCursor("Four")
                .HasActionUpdateCompleted("ContainerSecond")
                .HasActionUpdateCompleted("Root")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Completed
                        Four = Completed
            */

            CheckAllCompleted();
        }

        [TestMethod]
        public void ErrorOnFourRetryComplete()
        {
            /*
                Root = Waiting, Cursor
                    ContainerFirst = Waiting
                        First = Waiting
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            Run("Root")
                .HasActionClearingCursor("Root")
                .HasActionUpdateRunning("Root")
                .HasActionUpdateRunning("ContainerFirst")
                .HasActionSettingCursor("First")
                .HasActionUpdateToRun("First")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = ToRun, Cursor
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("First")
                .HasActionUpdateRunning("First")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Running, Cursor
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("First")
                .HasActionUpdateCompleted("First")
                .HasActionClearingCursor("First")
                .HasActionSettingCursor("Second")
                .HasActionUpdateToRun("Second")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Completed
                        Second = ToRun, Cursor
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("Second")
                .HasActionUpdateRunning("Second")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Completed
                        Second = Running, Cursor
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("Second")
                .HasActionUpdateCompleted("Second")
                .HasActionClearingCursor("Second")
                .HasActionUpdateCompleted("ContainerFirst")
                .HasActionSettingCursor("Third")
                .HasActionUpdateToRun("Third")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = ToRun, Cursor
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("Third")
                .HasActionUpdateRunning("Third")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Running, Cursor
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("Third")
                .HasActionUpdateCompleted("Third")
                .HasActionClearingCursor("Third")
                .HasActionUpdateRunning("ContainerSecond")
                .HasActionSettingCursor("Four")
                .HasActionUpdateToRun("Four")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Running
                        Four = ToRun, Cursor
            */

            SetRunning("Four")
                .HasActionUpdateRunning("Four")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Running
                        Four = Running, Cursor
            */


            SetError("Four")
                .HasActionUpdateError("Four")
                .HasActionUpdateError("ContainerSecond")
                .HasActionUpdateError("Root")
                .IsCheckedAll();

            /*
                Root = Error
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Error
                        Four = Error, Cursor
            */

            Run("Four")
                .HasActionUpdateRunning("Root")
                .HasActionUpdateRunning("ContainerSecond")
                .HasActionUpdateToRun("Four")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Running
                        Four = ToRun, Cursor
            */

            SetRunning("Four")
                .HasActionUpdateRunning("Four")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Running
                        Four = Running, Cursor
            */

            SetCompleted("Four")
                .HasActionUpdateCompleted("Four")
                .HasActionClearingCursor("Four")
                .HasActionUpdateCompleted("ContainerSecond")
                .HasActionUpdateCompleted("Root")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Completed
                        Four = Completed
            */

            CheckAllCompleted();
        }

        [TestMethod]
        public void StopOnFirstAndContainue()
        {
            /*
                Root = Waiting, Cursor
                    ContainerFirst = Waiting
                        First = Waiting
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            Run("Root")
                .HasActionClearingCursor("Root")
                .HasActionUpdateRunning("Root")
                .HasActionUpdateRunning("ContainerFirst")
                .HasActionSettingCursor("First")
                .HasActionUpdateToRun("First")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = ToRun, Cursor
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("First")
                .HasActionUpdateRunning("First")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Running, Cursor
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            Stop("First")
                .HasActionUpdateToStop("First")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = ToStop, Cursor
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetStopped("First")
                .HasActionUpdateStopped("First")
                .HasActionUpdateStopped("ContainerFirst")
                .HasActionUpdateStopped("Root")
                .IsCheckedAll();

            /*
                Root = Stopped
                    ContainerFirst = Stopped
                        First = Stopped, Cursor
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            Run("First")
                .HasActionUpdateRunning("ContainerFirst")
                .HasActionUpdateRunning("Root")
                .HasActionUpdateToRun("First")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = ToRun, Cursor
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("First")
                .HasActionUpdateRunning("First")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Running, Cursor
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("First")
                .HasActionUpdateCompleted("First")
                .HasActionClearingCursor("First")
                .HasActionSettingCursor("Second")
                .HasActionUpdateToRun("Second")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Completed
                        Second = ToRun, Cursor
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("Second")
                .HasActionUpdateRunning("Second")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Completed
                        Second = Running, Cursor
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("Second")
                .HasActionUpdateCompleted("Second")
                .HasActionClearingCursor("Second")
                .HasActionUpdateCompleted("ContainerFirst")
                .HasActionSettingCursor("Third")
                .HasActionUpdateToRun("Third")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = ToRun, Cursor
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("Third")
                .HasActionUpdateRunning("Third")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Running, Cursor
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("Third")
                .HasActionUpdateCompleted("Third")
                .HasActionClearingCursor("Third")
                .HasActionUpdateRunning("ContainerSecond")
                .HasActionSettingCursor("Four")
                .HasActionUpdateToRun("Four")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Running
                        Four = ToRun, Cursor
            */

            SetRunning("Four")
                .HasActionUpdateRunning("Four")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Running
                        Four = Running, Cursor
            */

            SetCompleted("Four")
                .HasActionUpdateCompleted("Four")
                .HasActionClearingCursor("Four")
                .HasActionUpdateCompleted("ContainerSecond")
                .HasActionUpdateCompleted("Root")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Completed
                        Four = Completed
            */

            CheckAllCompleted();
        }

        [TestMethod]
        public void StopOnSecondRetryComplete()
        {
            /*
                Root = Waiting, Cursor
                    ContainerFirst = Waiting
                        First = Waiting
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            Run("Root")
                .HasActionClearingCursor("Root")
                .HasActionUpdateRunning("Root")
                .HasActionUpdateRunning("ContainerFirst")
                .HasActionSettingCursor("First")
                .HasActionUpdateToRun("First")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = ToRun, Cursor
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("First")
                .HasActionUpdateRunning("First")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Running, Cursor
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("First")
                .HasActionUpdateCompleted("First")
                .HasActionClearingCursor("First")
                .HasActionSettingCursor("Second")
                .HasActionUpdateToRun("Second")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Completed
                        Second = ToRun, Cursor
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("Second")
                .HasActionUpdateRunning("Second")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Completed
                        Second = Running, Cursor
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */


            Stop("Second")
                .HasActionUpdateToStop("Second")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Completed
                        Second = ToStop, Cursor
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetStopped("Second")
                .HasActionUpdateStopped("Second")
                .HasActionUpdateStopped("ContainerFirst")
                .HasActionUpdateStopped("Root")
                .IsCheckedAll();

            /*
                Root = Stopped
                    ContainerFirst = Stopped
                        First = Completed
                        Second = Stopped, Cursor
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            Run("Second")
                .HasActionUpdateRunning("ContainerFirst")
                .HasActionUpdateRunning("Root")
                .HasActionUpdateToRun("Second")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Completed
                        Second = ToRun, Cursor
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("Second")
                .HasActionUpdateRunning("Second")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Completed
                        Second = Running, Cursor
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("Second")
                .HasActionUpdateCompleted("Second")
                .HasActionClearingCursor("Second")
                .HasActionUpdateCompleted("ContainerFirst")
                .HasActionSettingCursor("Third")
                .HasActionUpdateToRun("Third")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = ToRun, Cursor
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("Third")
                .HasActionUpdateRunning("Third")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Running, Cursor
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("Third")
                .HasActionUpdateCompleted("Third")
                .HasActionClearingCursor("Third")
                .HasActionUpdateRunning("ContainerSecond")
                .HasActionSettingCursor("Four")
                .HasActionUpdateToRun("Four")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Running
                        Four = ToRun, Cursor
            */

            SetRunning("Four")
                .HasActionUpdateRunning("Four")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Running
                        Four = Running, Cursor
            */

            SetCompleted("Four")
                .HasActionUpdateCompleted("Four")
                .HasActionClearingCursor("Four")
                .HasActionUpdateCompleted("ContainerSecond")
                .HasActionUpdateCompleted("Root")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Completed
                        Four = Completed
            */

            CheckAllCompleted();
        }

        [TestMethod]
        public void StopOnThirdRetryComplete()
        {
            /*
                Root = Waiting, Cursor
                    ContainerFirst = Waiting
                        First = Waiting
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            Run("Root")
                .HasActionClearingCursor("Root")
                .HasActionUpdateRunning("Root")
                .HasActionUpdateRunning("ContainerFirst")
                .HasActionSettingCursor("First")
                .HasActionUpdateToRun("First")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = ToRun, Cursor
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("First")
                .HasActionUpdateRunning("First")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Running, Cursor
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("First")
                .HasActionUpdateCompleted("First")
                .HasActionClearingCursor("First")
                .HasActionSettingCursor("Second")
                .HasActionUpdateToRun("Second")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Completed
                        Second = ToRun, Cursor
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("Second")
                .HasActionUpdateRunning("Second")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Completed
                        Second = Running, Cursor
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("Second")
                .HasActionUpdateCompleted("Second")
                .HasActionClearingCursor("Second")
                .HasActionUpdateCompleted("ContainerFirst")
                .HasActionSettingCursor("Third")
                .HasActionUpdateToRun("Third")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = ToRun, Cursor
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("Third")
                .HasActionUpdateRunning("Third")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Running, Cursor
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            Stop("Third")
                .HasActionUpdateToStop("Third")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = ToStop, Cursor
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetStopped("Third")
                .HasActionUpdateStopped("Third")
                .HasActionUpdateStopped("Root")
                .IsCheckedAll();

            /*
                Root = Stopped
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Stopped, Cursor
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            Run("Third")
                .HasActionUpdateRunning("Root")
                .HasActionUpdateToRun("Third")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = ToRun, Cursor
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("Third")
                .HasActionUpdateRunning("Third")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Running, Cursor
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("Third")
                .HasActionUpdateCompleted("Third")
                .HasActionClearingCursor("Third")
                .HasActionUpdateRunning("ContainerSecond")
                .HasActionSettingCursor("Four")
                .HasActionUpdateToRun("Four")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Running
                        Four = ToRun, Cursor
            */

            SetRunning("Four")
                .HasActionUpdateRunning("Four")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Running
                        Four = Running, Cursor
            */

            SetCompleted("Four")
                .HasActionUpdateCompleted("Four")
                .HasActionClearingCursor("Four")
                .HasActionUpdateCompleted("ContainerSecond")
                .HasActionUpdateCompleted("Root")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Completed
                        Four = Completed
            */

            CheckAllCompleted();
        }

        [TestMethod]
        public void StopOnFourRetryComplete()
        {
            /*
                Root = Waiting, Cursor
                    ContainerFirst = Waiting
                        First = Waiting
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            Run("Root")
                .HasActionClearingCursor("Root")
                .HasActionUpdateRunning("Root")
                .HasActionUpdateRunning("ContainerFirst")
                .HasActionSettingCursor("First")
                .HasActionUpdateToRun("First")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = ToRun, Cursor
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("First")
                .HasActionUpdateRunning("First")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Running, Cursor
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("First")
                .HasActionUpdateCompleted("First")
                .HasActionClearingCursor("First")
                .HasActionSettingCursor("Second")
                .HasActionUpdateToRun("Second")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Completed
                        Second = ToRun, Cursor
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("Second")
                .HasActionUpdateRunning("Second")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Completed
                        Second = Running, Cursor
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("Second")
                .HasActionUpdateCompleted("Second")
                .HasActionClearingCursor("Second")
                .HasActionUpdateCompleted("ContainerFirst")
                .HasActionSettingCursor("Third")
                .HasActionUpdateToRun("Third")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = ToRun, Cursor
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("Third")
                .HasActionUpdateRunning("Third")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Running, Cursor
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("Third")
                .HasActionUpdateCompleted("Third")
                .HasActionClearingCursor("Third")
                .HasActionUpdateRunning("ContainerSecond")
                .HasActionSettingCursor("Four")
                .HasActionUpdateToRun("Four")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Running
                        Four = ToRun, Cursor
            */

            SetRunning("Four")
                .HasActionUpdateRunning("Four")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Running
                        Four = Running, Cursor
            */


            Stop("Four")
                .HasActionUpdateToStop("Four")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Running
                        Four = ToStop, Cursor
            */

            SetStopped("Four")
                .HasActionUpdateStopped("Four")
                .HasActionUpdateStopped("ContainerSecond")
                .HasActionUpdateStopped("Root")
                .IsCheckedAll();

            /*
                Root = Stopped
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Stopped
                        Four = Stopped, Cursor
            */

            Run("Four")
                .HasActionUpdateRunning("Root")
                .HasActionUpdateRunning("ContainerSecond")
                .HasActionUpdateToRun("Four")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Running
                        Four = ToRun, Cursor
            */

            SetRunning("Four")
                .HasActionUpdateRunning("Four")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Running
                        Four = Running, Cursor
            */

            SetCompleted("Four")
                .HasActionUpdateCompleted("Four")
                .HasActionClearingCursor("Four")
                .HasActionUpdateCompleted("ContainerSecond")
                .HasActionUpdateCompleted("Root")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Completed
                        Four = Completed
            */

            CheckAllCompleted();
        }

        [TestMethod]
        public void BreakPointOnFirstAndComplete()
        {
            SetBreakPoint("First")
                .HasActionSettingBreakPoint("First")
                .IsCheckedAll();

            /*
                Root = Waiting, Cursor
                    ContainerFirst = Waiting
                        First = Waiting
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            Run("Root")
                .HasActionClearingCursor("Root")
                .HasActionUpdateStopped("Root")
                .HasActionUpdateStopped("ContainerFirst")
                .HasActionSettingCursor("First")
                .HasActionUpdateStopped("First")
                .IsCheckedAll();

            /*
                Root = Stopped
                    ContainerFirst = Stopped
                        First = Stopped, Cursor
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            Run("First")
                .HasActionUpdateRunning("ContainerFirst")
                .HasActionUpdateRunning("Root")
                .HasActionUpdateToRun("First")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = ToRun, Cursor
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("First")
                .HasActionUpdateRunning("First")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Running, Cursor
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("First")
                .HasActionUpdateCompleted("First")
                .HasActionClearingCursor("First")
                .HasActionSettingCursor("Second")
                .HasActionUpdateToRun("Second")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Completed
                        Second = ToRun, Cursor
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("Second")
                .HasActionUpdateRunning("Second")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Completed
                        Second = Running, Cursor
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("Second")
                .HasActionUpdateCompleted("Second")
                .HasActionClearingCursor("Second")
                .HasActionUpdateCompleted("ContainerFirst")
                .HasActionSettingCursor("Third")
                .HasActionUpdateToRun("Third")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = ToRun, Cursor
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("Third")
                .HasActionUpdateRunning("Third")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Running, Cursor
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("Third")
                .HasActionUpdateCompleted("Third")
                .HasActionClearingCursor("Third")
                .HasActionUpdateRunning("ContainerSecond")
                .HasActionSettingCursor("Four")
                .HasActionUpdateToRun("Four")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Running
                        Four = ToRun, Cursor
            */

            SetRunning("Four")
                .HasActionUpdateRunning("Four")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Running
                        Four = Running, Cursor
            */

            SetCompleted("Four")
                .HasActionUpdateCompleted("Four")
                .HasActionClearingCursor("Four")
                .HasActionUpdateCompleted("ContainerSecond")
                .HasActionUpdateCompleted("Root")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Completed
                        Four = Completed
            */

            CheckAllCompleted();
        }

        [TestMethod]
        public void BreakPointOnSecondAndComplete()
        {
            SetBreakPoint("Second")
                .HasActionSettingBreakPoint("Second")
                .IsCheckedAll();

            /*
                Root = Waiting, Cursor
                    ContainerFirst = Waiting
                        First = Waiting
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            Run("Root")
                .HasActionClearingCursor("Root")
                .HasActionUpdateRunning("Root")
                .HasActionUpdateRunning("ContainerFirst")
                .HasActionSettingCursor("First")
                .HasActionUpdateToRun("First")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = ToRun, Cursor
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("First")
                .HasActionUpdateRunning("First")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Running, Cursor
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("First")
                .HasActionUpdateCompleted("First")
                .HasActionClearingCursor("First")
                .HasActionSettingCursor("Second")
                .HasActionUpdateStopped("Second")
                .HasActionUpdateStopped("ContainerFirst")
                .HasActionUpdateStopped("Root")
                .IsCheckedAll();

            /*
                Root = Stopped
                    ContainerFirst = Stopped
                        First = Completed
                        Second = Stopped, Cursor
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            Run("Second")
                .HasActionUpdateRunning("ContainerFirst")
                .HasActionUpdateRunning("Root")
                .HasActionUpdateToRun("Second")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Completed
                        Second = ToRun, Cursor
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("Second")
                .HasActionUpdateRunning("Second")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Completed
                        Second = Running, Cursor
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("Second")
                .HasActionUpdateCompleted("Second")
                .HasActionClearingCursor("Second")
                .HasActionUpdateCompleted("ContainerFirst")
                .HasActionSettingCursor("Third")
                .HasActionUpdateToRun("Third")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = ToRun, Cursor
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("Third")
                .HasActionUpdateRunning("Third")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Running, Cursor
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("Third")
                .HasActionUpdateCompleted("Third")
                .HasActionClearingCursor("Third")
                .HasActionUpdateRunning("ContainerSecond")
                .HasActionSettingCursor("Four")
                .HasActionUpdateToRun("Four")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Running
                        Four = ToRun, Cursor
            */

            SetRunning("Four")
                .HasActionUpdateRunning("Four")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Running
                        Four = Running, Cursor
            */

            SetCompleted("Four")
                .HasActionUpdateCompleted("Four")
                .HasActionClearingCursor("Four")
                .HasActionUpdateCompleted("ContainerSecond")
                .HasActionUpdateCompleted("Root")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Completed
                        Four = Completed
            */

            CheckAllCompleted();
        }

        [TestMethod]
        public void BreakPointOnThirdAndComplete()
        {
            SetBreakPoint("Third")
                .HasActionSettingBreakPoint("Third")
                .IsCheckedAll();

            /*
                Root = Waiting, Cursor
                    ContainerFirst = Waiting
                        First = Waiting
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            Run("Root")
                .HasActionClearingCursor("Root")
                .HasActionUpdateRunning("Root")
                .HasActionUpdateRunning("ContainerFirst")
                .HasActionSettingCursor("First")
                .HasActionUpdateToRun("First")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = ToRun, Cursor
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("First")
                .HasActionUpdateRunning("First")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Running, Cursor
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("First")
                .HasActionUpdateCompleted("First")
                .HasActionClearingCursor("First")
                .HasActionSettingCursor("Second")
                .HasActionUpdateToRun("Second")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Completed
                        Second = ToRun, Cursor
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("Second")
                .HasActionUpdateRunning("Second")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Completed
                        Second = Running, Cursor
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("Second")
                .HasActionUpdateCompleted("Second")
                .HasActionClearingCursor("Second")
                .HasActionUpdateCompleted("ContainerFirst")
                .HasActionSettingCursor("Third")
                .HasActionUpdateStopped("Third")
                .HasActionUpdateStopped("Root")
                .IsCheckedAll();

            /*
                Root = Stopped
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Stopped, Cursor
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            Run("Third")
                .HasActionUpdateRunning("Root")
                .HasActionUpdateToRun("Third")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = ToRun, Cursor
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("Third")
                .HasActionUpdateRunning("Third")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Running, Cursor
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("Third")
                .HasActionUpdateCompleted("Third")
                .HasActionClearingCursor("Third")
                .HasActionUpdateRunning("ContainerSecond")
                .HasActionSettingCursor("Four")
                .HasActionUpdateToRun("Four")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Running
                        Four = ToRun, Cursor
            */

            SetRunning("Four")
                .HasActionUpdateRunning("Four")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Running
                        Four = Running, Cursor
            */

            SetCompleted("Four")
                .HasActionUpdateCompleted("Four")
                .HasActionClearingCursor("Four")
                .HasActionUpdateCompleted("ContainerSecond")
                .HasActionUpdateCompleted("Root")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Completed
                        Four = Completed
            */

            CheckAllCompleted();
        }

        [TestMethod]
        public void BreakPointOnFourAndComplete()
        {
            SetBreakPoint("Four")
                .HasActionSettingBreakPoint("Four")
                .IsCheckedAll();

            /*
                Root = Waiting, Cursor
                    ContainerFirst = Waiting
                        First = Waiting
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            Run("Root")
                .HasActionClearingCursor("Root")
                .HasActionUpdateRunning("Root")
                .HasActionUpdateRunning("ContainerFirst")
                .HasActionSettingCursor("First")
                .HasActionUpdateToRun("First")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = ToRun, Cursor
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("First")
                .HasActionUpdateRunning("First")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Running, Cursor
                        Second = Waiting
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("First")
                .HasActionUpdateCompleted("First")
                .HasActionClearingCursor("First")
                .HasActionSettingCursor("Second")
                .HasActionUpdateToRun("Second")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Completed
                        Second = ToRun, Cursor
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("Second")
                .HasActionUpdateRunning("Second")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Running
                        First = Completed
                        Second = Running, Cursor
                    Third = Waiting
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("Second")
                .HasActionUpdateCompleted("Second")
                .HasActionClearingCursor("Second")
                .HasActionUpdateCompleted("ContainerFirst")
                .HasActionSettingCursor("Third")
                .HasActionUpdateToRun("Third")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = ToRun, Cursor
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetRunning("Third")
                .HasActionUpdateRunning("Third")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Running, Cursor
                    ContainerSecond = Waiting
                        Four = Waiting
            */

            SetCompleted("Third")
                .HasActionUpdateCompleted("Third")
                .HasActionClearingCursor("Third")
                .HasActionUpdateStopped("ContainerSecond")
                .HasActionSettingCursor("Four")
                .HasActionUpdateStopped("Four")
                .HasActionUpdateStopped("Root")
                .IsCheckedAll();

            /*
                Root = Stopped
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Stopped
                        Four = Stopped, Cursor
            */

            Run("Four")
                .HasActionUpdateRunning("Root")
                .HasActionUpdateRunning("ContainerSecond")
                .HasActionUpdateToRun("Four")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Running
                        Four = ToRun, Cursor
            */

            SetRunning("Four")
                .HasActionUpdateRunning("Four")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Running
                        Four = Running, Cursor
            */

            SetCompleted("Four")
                .HasActionUpdateCompleted("Four")
                .HasActionClearingCursor("Four")
                .HasActionUpdateCompleted("ContainerSecond")
                .HasActionUpdateCompleted("Root")
                .IsCheckedAll();

            /*
                Root = Running
                    ContainerFirst = Completed
                        First = Completed
                        Second = Completed
                    Third = Completed
                    ContainerSecond = Completed
                        Four = Completed
            */

            CheckAllCompleted();
        }
    }
}
