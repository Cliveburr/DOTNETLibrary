﻿using Runner.Business.Actions;
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
    public class FlowSequentialTest : TestActionsBase
    {
        private ActionControl GetControl()
        {
            var flow = new Flow
            {
                Name = "Test",
                AgentPool = "",
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
                        }
                    },
                    Next = new List<FlowActionContainer>
                    {
                        new FlowActionContainer
                        {
                            Label = "After",
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

            SetRunning(control, "Root");
            SetCompletedOnSameContainer(control, "Root");

            SetRunning(control, "Root");
            SetCompletedAndMoveToNextContainer(control, "Root");

            SetRunning(control, "After");
            SetCompletedOnSameContainer(control, "After");

            SetRunning(control, "After");
            SetCompletedAndDone(control, "After");
        }

        // roda uma execução com erro, retry e vai até o final
        [TestMethod]
        public void ErrorAndGo()
        {
            var control = GetControl();

            SetRunning(control, "Root");
            SetCompletedOnSameContainer(control, "Root");

            SetRunning(control, "Root");
            SetError(control, "Root");

            SetRunning(control, "Root");
            SetCompletedAndMoveToNextContainer(control, "Root");

            SetRunning(control, "After");
            SetCompletedOnSameContainer(control, "After");

            SetRunning(control, "After");
            SetCompletedAndDone(control, "After");
        }

        // roda uma execução com breakpoint na action 1
        [TestMethod]
        public void BreakPointAndGo()
        {
            var control = GetControl();

            control.Run.Actions[1].BreakPoint = true;

            SetRunning(control, "Root");
            SetCompletedAndBreak(control, "Root");

            SetRunning(control, "Root");
            SetCompletedAndMoveToNextContainer(control, "Root");

            SetRunning(control, "After");
            SetCompletedOnSameContainer(control, "After");

            SetRunning(control, "After");
            SetCompletedAndDone(control, "After");
        }


        // roda uma execução com breakpoint na action 1 e 2
        [TestMethod]
        public void DoubleBreakPointAndGo()
        {
            var control = GetControl();

            control.Run.Actions[1].BreakPoint = true;
            control.Run.Actions[2].BreakPoint = true;

            SetRunning(control, "Root");
            SetCompletedAndBreak(control, "Root");

            SetRunning(control, "Root");
            SetCompletedAndMoveToNextContainerWithBreak1(control, "Root");

            SetRunning(control, "After");
            SetCompletedOnSameContainer(control, "After");

            SetRunning(control, "After");
            SetCompletedAndDone(control, "After");
        }
    }
}