using MongoDB.Bson;
using Runner.Business.Actions;
using Runner.Business.Entities.Nodes.Types;
using Runner.Business.Tests.Helpers;

namespace Runner.Business.Tests.Actions
{
    [TestClass]
    public class ParallelWithContainerTests : TestActionsBase
    {
        protected override ActionControl GetControl()
        {
            var flow = new Flow
            {
                NodeId = ObjectId.Empty,
                Input = [],
                Root = new FlowAction
                {
                    Label = "Root",
                    Type = ActionType.Parallel,
                    Childs = new List<FlowAction>
                    {
                        new FlowAction
                        {
                            Label = "Cont",
                            Type = ActionType.Container,
                            Childs = new List<FlowAction>
                            {
                                new FlowAction
                                {
                                    Label = "Hello",
                                    Type = ActionType.Script
                                },
                                new FlowAction
                                {
                                    Label = "Delay1",
                                    Type = ActionType.Script
                                },
                                new FlowAction
                                {
                                    Label = "Delay2",
                                    Type = ActionType.Script
                                }
                            }
                        },
                        new FlowAction
                        {
                            Label = "Other",
                            Type = ActionType.Script
                        }
                    }
                }
            };

            var run = ActionControl.Build(flow);

            return ActionControl.From(run);
        }


        [TestMethod]
        public void CreateAndRunComplete()
        {
            /*
                Root = Waiting, Cursor
                    Cont = Waiting
                        Hello = Waiting
                        Delay1 = Waiting
                        Delay2 = Waiting
                    Other = Waiting
            */

            Run("Root")
                .HasActionClearingCursor("Root")
                .HasActionUpdateRunning("Root")
                    .HasActionUpdateRunning("Cont")
                    .HasActionSettingCursor("Hello")
                    .HasActionUpdateToRun("Hello")
                .HasActionSettingCursor("Other")
                .HasActionUpdateToRun("Other")
                .IsCheckedAll();

            /*
                Root = Running
                    Cont = Running
                        Hello = ToRun, Cursor
                        Delay1 = Waiting
                        Delay2 = Waiting
                    Other = ToRun, Cursor
            */

            SetRunning("Hello")
                .HasActionUpdateRunning("Hello")
                .IsCheckedAll();

            /*
                Root = Running
                    Cont = Running
                        Hello = Running, Cursor
                        Delay1 = Waiting
                        Delay2 = Waiting
                    Other = ToRun, Cursor
            */

            //CheckAllCompleted();
        }
    }
}
