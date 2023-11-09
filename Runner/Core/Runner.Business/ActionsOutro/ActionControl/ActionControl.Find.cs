using Runner.Business.Actions;
using Runner.Business.ActionsOutro.Types;
using Runner.Business.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Cursor FindCursor(int actionId)
        {
            var cursor = EntityRun.Cursors
                .FirstOrDefault(c => c.ActionId == actionId);
            Assert.MustNotNull(cursor, $"Cursor not found for Action: {actionId}");
            return cursor;
        }

        public ActionTypesBase FindActionType(Action action)
        {
            switch (action.Type)
            {
                case ActionType.Script: return new ActionScript(this, action);
                default: throw new RunnerException($"Invalid ActionType: {action.Type}! Action: {action.ActionId}");
            }
        }
    }
}
