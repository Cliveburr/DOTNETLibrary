using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Actions
{
    public partial class ActionControl
    {
        public ActionContainer Root
        {
            get
            {
                var container = Run.Containers
                    .FirstOrDefault(c => c.ActionContainerId == Run.RootContainerId);
                Assert.MustNotNull(container, $"Root Container not found! RootContainerId: {Run.RootContainerId}");
                return container;
            }
        }

        public ActionContainer FindContainer(int actionContainerId)
        {
            var container = Run.Containers
                .FirstOrDefault(c => c.ActionContainerId == actionContainerId);
            Assert.MustNotNull(container, $"Container not found! ActionContainerId: {actionContainerId}");
            return container;
        }

        public Action FindPositionAction(ActionContainer actionContainer)
        {
            Assert.MustTrue(actionContainer.Position < actionContainer.Actions.Count, $"Container with wrong Position! Position: {actionContainer.Position}, Actions: {actionContainer.Actions.Count}, Container: {actionContainer.ActionContainerId}-{actionContainer.Label}");
            var actionId = actionContainer.Actions[actionContainer.Position];
            var action = Run.Actions
                .FirstOrDefault(a => a.ActionId == actionId);
            Assert.MustNotNull(action, $"Action not found! ActionId: {actionId}");

            return action;
        }

        public (ActionContainer Container, Action Action) FindPositionActionOnContainer(int actionContainerId)
        {
            var container = FindContainer(actionContainerId);
            var action = FindPositionAction(container);
            return (container, action);
        }
    }
}
