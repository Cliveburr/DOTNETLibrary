using Runner.Business.Actions.Types;
using Runner.Business.Entities.Nodes.Types;

namespace Runner.Business.Actions
{
    public partial class ActionControl
    {
        public Action FindAction(int actionId)
        {
            var action = EntityRun.Actions
                .FirstOrDefault(a => a.ActionId == actionId);
            Assert.MustNotNull(action, $"Action not found! Action: {actionId}");
            return action;
        }

        public Action? FindAction(string actionLabel)
        {
            return EntityRun.Actions
                .FirstOrDefault(a => a.Label == actionLabel);
        }

        public ActionTypesBase FindActionType(Action action)
        {
            switch (action.Type)
            {
                case ActionType.Script: return new ActionScript(action);
                case ActionType.Container: return new ActionContainer(action);
                case ActionType.Parallel: return new ActionParallel(action);
                default: throw new RunnerException($"Invalid ActionType: {action.Type}! Action: {action.ActionId}");
            }
        }

        public (Action Action, ActionTypesBase ActionType) FindActionAndType(int actionId)
        {
            var action = FindAction(actionId);
            var actionType = FindActionType(action);
            return (action, actionType);
        }
    }
}
