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
                case ActionType.ParentRun: return new ActionParentRun(action);
                default: throw new RunnerException($"Invalid ActionType: {action.Type}! Action: {action.ActionId}");
            }
        }

        public (Action Action, ActionTypesBase ActionType) FindActionAndType(int actionId)
        {
            var action = FindAction(actionId);
            var actionType = FindActionType(action);
            return (action, actionType);
        }

        public IEnumerable<Action> FindActionsAbleToRun(Action action)
        {
            if (action.WithCursor && !action.BreakPoint && (
                action.Status == ActionStatus.Waiting ||
                action.Status == ActionStatus.Stopped ||
                action.Status == ActionStatus.Error))
            {
                yield return action;
            }

            if (action.Childs is not null)
            {
                foreach (var childId in action.Childs)
                {
                    var childAction = FindAction(childId);
                    foreach (var ret in FindActionsAbleToRun(childAction))
                    {
                        yield return ret;
                    }
                }
            }
        }
    }
}
