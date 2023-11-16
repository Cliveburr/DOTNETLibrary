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
    public class EmptyContainerTest : TestActionsBase
    {
        protected override ActionControl GetControl()
        {
            var flow = new Flow
            {
                Name = "Test",
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
