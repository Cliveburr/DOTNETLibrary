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

        public abstract List<CommandEffect> Run();
        public abstract List<CommandEffect> BackRun();
        public abstract List<CommandEffect> SetRunning();
        public abstract List<CommandEffect> BackSetRunning();
        public abstract List<CommandEffect> SetCompleted();
        public abstract List<CommandEffect> BackSetCompleted();
        public abstract List<CommandEffect> SetError();
        public abstract List<CommandEffect> BackSetError();
        public abstract List<CommandEffect> Stop();
        public abstract List<CommandEffect> BackStop();
        public abstract List<CommandEffect> SetStopped();
        public abstract List<CommandEffect> BackSetStopped();

        protected List<CommandEffect> PropagateBackRun(int actionId)
        {
            var action = _control.FindAction(actionId);

            var actionType = _control.FindActionType(action);

            return actionType.BackRun();
        }

        protected List<CommandEffect> PropagateBackSetRunning(int actionId)
        {
            var action = _control.FindAction(actionId);

            var actionType = _control.FindActionType(action);

            return actionType.BackSetRunning();
        }

        protected List<CommandEffect> PropagateBackSetCompleted(int actionId)
        {
            var action = _control.FindAction(actionId);

            var actionType = _control.FindActionType(action);

            return actionType.BackSetCompleted();
        }


        protected List<CommandEffect> PropagateBackSetError(int actionId)
        {
            var action = _control.FindAction(actionId);

            var actionType = _control.FindActionType(action);

            return actionType.BackSetError();
        }

        protected List<CommandEffect> PropagateBackStop(int actionId)
        {
            var action = _control.FindAction(actionId);

            var actionType = _control.FindActionType(action);

            return actionType.BackStop();
        }

        protected List<CommandEffect> PropagateBackSetStoppped(int actionId)
        {
            var action = _control.FindAction(actionId);

            var actionType = _control.FindActionType(action);

            return actionType.BackSetStopped();
        }
    }
}
