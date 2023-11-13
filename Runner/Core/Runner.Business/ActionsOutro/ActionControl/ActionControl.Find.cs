using Runner.Business.Actions;
using Runner.Business.ActionsOutro.Types;
using Runner.Business.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActionContainer = Runner.Business.ActionsOutro.Types.ActionContainer;

namespace Runner.Business.ActionsOutro
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
