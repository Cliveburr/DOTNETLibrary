using MongoDB.Bson;
using Runner.Business.Actions;
using Runner.Business.Entities.Nodes.Types;
using Runner.Business.Tests.Helpers;

namespace Runner.Business.Tests.Actions
{
    [TestClass]
    public class EmptyContainerTest : TestActionsBase
    {
        protected override ActionControl GetControl()
        {
            var flow = new Flow
            {
                NodeId = ObjectId.Empty,
                Root = new FlowAction
                {
                    Label = "Container",
                    Type = ActionType.Container,
                    Childs = new List<FlowAction>
                    {
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
                .HasActionClearingCursor("Container")
                .HasActionUpdateCompleted("Container")
                .IsCheckedAll();
        }
    }
}
