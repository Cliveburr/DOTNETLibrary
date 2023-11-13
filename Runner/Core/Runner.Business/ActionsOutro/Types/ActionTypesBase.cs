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

        public abstract IEnumerable<CommandEffect> ContinueRun();
        public abstract IEnumerable<CommandEffect> Run();
        public abstract IEnumerable<CommandEffect> BackRun();
        public abstract IEnumerable<CommandEffect> SetRunning();
        public abstract IEnumerable<CommandEffect> BackSetRunning();
        public abstract IEnumerable<CommandEffect> SetCompleted();
        public abstract IEnumerable<CommandEffect> BackSetCompleted(int actionChildId);
        public abstract IEnumerable<CommandEffect> SetError();
        public abstract IEnumerable<CommandEffect> BackSetError();
        public abstract IEnumerable<CommandEffect> Stop();
        public abstract IEnumerable<CommandEffect> BackStop();
        public abstract IEnumerable<CommandEffect> SetStopped();
        public abstract IEnumerable<CommandEffect> BackSetStopped();
        public abstract IEnumerable<CommandEffect> SetBreakPoint();
        public abstract IEnumerable<CommandEffect> CleanBreakPoint();
        public abstract IEnumerable<CommandEffect> BackBreakPoint();

        protected IEnumerable<CommandEffect> PropagateBackRun()
        {
            if (_action.Parent.HasValue)
            {
                var (action, actionType) = _control.FindActionAndType(_action.Parent.Value);

                foreach (var command in actionType.BackRun())
                {
                    yield return command;
                };
            }
        }

        protected IEnumerable<CommandEffect> PropagateBackSetRunning()
        {
            if (_action.Parent.HasValue)
            {
                var (action, actionType) = _control.FindActionAndType(_action.Parent.Value);

                foreach (var command in actionType.BackSetRunning())
                {
                    yield return command;
                };
            }
        }

        protected IEnumerable<CommandEffect> PropagateBackSetCompleted()
        {
            if (_action.Parent.HasValue)
            {
                var (action, actionType) = _control.FindActionAndType(_action.Parent.Value);

                foreach (var command in actionType.BackSetCompleted(_action.ActionId))
                {
                    yield return command;
                };
            }
        }


        protected IEnumerable<CommandEffect> PropagateBackSetError()
        {
            if (_action.Parent.HasValue)
            {
                var (action, actionType) = _control.FindActionAndType(_action.Parent.Value);

                foreach (var command in actionType.BackSetError())
                {
                    yield return command;
                };
            }
        }

        protected IEnumerable<CommandEffect> PropagateBackStop()
        {
            if (_action.Parent.HasValue)
            {
                var (action, actionType) = _control.FindActionAndType(_action.Parent.Value);

                foreach (var command in actionType.BackStop())
                {
                    yield return command;
                };
            }
        }

        protected IEnumerable<CommandEffect> PropagateBackSetStoppped()
        {
            if (_action.Parent.HasValue)
            {
                var (action, actionType) = _control.FindActionAndType(_action.Parent.Value);

                foreach (var command in actionType.BackSetStopped())
                {
                    yield return command;
                };
            }
        }

        protected IEnumerable<CommandEffect> PropagateBackBreakPoint()
        {
            if (_action.Parent.HasValue)
            {
                var (action, actionType) = _control.FindActionAndType(_action.Parent.Value);

                foreach (var command in actionType.BackBreakPoint())
                {
                    yield return command;
                };
            }
        }
    }
}
