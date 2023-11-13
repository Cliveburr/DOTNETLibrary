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
    public class EmptyContainerTest : TestActionsBase
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
