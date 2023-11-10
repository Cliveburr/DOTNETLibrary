using Runner.Business.Actions;
using Runner.Business.ActionsOutro.Types;
using Runner.Business.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
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

        public Cursor FindCursor(int actionId)
        {
            var cursor = EntityRun.Cursors
                .FirstOrDefault(c => c.ActionId == actionId);
            Assert.MustNotNull(cursor, $"Cursor not found for Action: {actionId}");
            return cursor;
        }

        public IEnumerable<Cursor> FindCursorOnPasseds(int actionId)
        {
            return EntityRun.Cursors
                .Where(c => c.ActionsPasseds.Contains(actionId));
        }

        public (Cursor MyCursor, Action CurrentAction) FindCurrentActionOnMyCursor(int actionId)
        {
            var cursorHasPassedOverMe = FindCursorOnPasseds(actionId)
                .ToList();
            Assert.Enumerable.CountEquals(cursorHasPassedOverMe, 1, $"Wrong cursor state for Action! {actionId}");

            var myCursor = cursorHasPassedOverMe[0];
            var actionIdOnCursor = myCursor.ActionId;
            var currentAction = FindAction(actionIdOnCursor);
            return (myCursor, currentAction);
        }

        public ActionTypesBase FindActionType(Action action)
        {
            switch (action.Type)
            {
                case ActionType.Script: return new ActionScript(this, action);
                case ActionType.Container: return new ActionContainer(this, action);
                default: throw new RunnerException($"Invalid ActionType: {action.Type}! Action: {action.ActionId}");
            }
        }
    }
}
