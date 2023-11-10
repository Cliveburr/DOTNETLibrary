using Runner.Business.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.ActionsOutro.Types
{
    public abstract class ActionTypesBase
    {
        protected readonly ActionControl _control;
        protected readonly Action _action;

        public ActionTypesBase(ActionControl control, Action action)
        {
            _control = control;
            _action = action;
        }

        public abstract IEnumerable<CommandEffect> Run();
        public abstract IEnumerable<CommandEffect> BackRun();
        public abstract IEnumerable<CommandEffect> SetRunning();
        public abstract IEnumerable<CommandEffect> BackSetRunning();
        public abstract IEnumerable<CommandEffect> SetCompleted();
        public abstract IEnumerable<CommandEffect> BackSetCompleted();
        public abstract IEnumerable<CommandEffect> SetError();
        public abstract IEnumerable<CommandEffect> BackSetError();
        public abstract IEnumerable<CommandEffect> Stop();
        public abstract IEnumerable<CommandEffect> BackStop();
        public abstract IEnumerable<CommandEffect> SetStopped();
        public abstract IEnumerable<CommandEffect> BackSetStopped();
        public abstract IEnumerable<CommandEffect> SetBreakPoint();
        public abstract IEnumerable<CommandEffect> CleanBreakPoint();
        public abstract IEnumerable<CommandEffect> BackBreakPoint();

        protected IEnumerable<CommandEffect> PropagateBackRun(int actionId)
        {
            var action = _control.FindAction(actionId);
            var actionType = _control.FindActionType(action);

            return actionType.BackRun();
        }

        protected IEnumerable<CommandEffect> PropagateBackSetRunning(int actionId)
        {
            var action = _control.FindAction(actionId);
            var actionType = _control.FindActionType(action);

            return actionType.BackSetRunning();
        }

        protected IEnumerable<CommandEffect> PropagateBackSetCompleted(int actionId)
        {
            var action = _control.FindAction(actionId);
            var actionType = _control.FindActionType(action);

            return actionType.BackSetCompleted();
        }


        protected IEnumerable<CommandEffect> PropagateBackSetError(int actionId)
        {
            var action = _control.FindAction(actionId);
            var actionType = _control.FindActionType(action);

            return actionType.BackSetError();
        }

        protected IEnumerable<CommandEffect> PropagateBackStop(int actionId)
        {
            var action = _control.FindAction(actionId);
            var actionType = _control.FindActionType(action);

            return actionType.BackStop();
        }

        protected IEnumerable<CommandEffect> PropagateBackSetStoppped(int actionId)
        {
            var action = _control.FindAction(actionId);
            var actionType = _control.FindActionType(action);

            return actionType.BackSetStopped();
        }

        protected IEnumerable<CommandEffect> MoveCursorFoward(Cursor cursor, int nextActionId)
        {
            cursor.ActionsPasseds.Add(cursor.ActionId);
            cursor.ActionId = nextActionId;

            yield return new CommandEffect(ComandEffectType.CursorUpdate, cursor);
        }

        protected IEnumerable<CommandEffect> PropagateBackBreakPoint(int actionId)
        {
            var action = _control.FindAction(actionId);
            var actionType = _control.FindActionType(action);

            return actionType.BackBreakPoint();
        }
    }
}
