using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.ActionsOutro.Types
{
    public class ActionScript : ActionTypesBase
    {
        public ActionScript(ActionControl control, Action action)
            : base(control, action)
        {
        }

        public override IEnumerable<CommandEffect> Run()
        {
            _ = _control.FindCursor(_action.ActionId); // just to confirm cursor on it

            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Waiting,
                ActionStatus.Stopped,
                ActionStatus.Error
            }, $"Action in wrong status to Run! {_action.ActionId}-{_action.Label}");

            _action.Status = ActionStatus.ToRun;
            yield return new CommandEffect(ComandEffectType.ActionUpdateToRun, _action);

            if (_action.Parent.HasValue)
            {
                foreach (var command in PropagateBackRun(_action.Parent.Value))
                {
                    yield return command;
                };
            }
        }

        public override IEnumerable<CommandEffect> BackRun()
        {
            throw new RunnerException("ActionScript shound't never run from back!");
        }

        public override IEnumerable<CommandEffect> SetRunning()
        {
            _ = _control.FindCursor(_action.ActionId); // just to confirm cursor on it

            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.ToRun
            }, $"Action in wrong status to Running! {_action.ActionId}-{_action.Label}");

            _action.Status = ActionStatus.Running;
            yield return new CommandEffect(ComandEffectType.ActionUpdateStatus, _action);

            if (_action.Parent.HasValue)
            {
                foreach (var command in PropagateBackSetRunning(_action.Parent.Value))
                {
                    yield return command;
                };
            }
        }

        public override IEnumerable<CommandEffect> BackSetRunning()
        {
            throw new RunnerException("ActionScript shound't never running from back!");
        }

        public override IEnumerable<CommandEffect> SetCompleted()
        {
            _ = _control.FindCursor(_action.ActionId); // just to confirm cursor on it

            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Running
            }, $"Action in wrong status to Completed! {_action.ActionId}-{_action.Label}");

            _action.Status = ActionStatus.Completed;
            yield return new CommandEffect(ComandEffectType.ActionUpdateStatus, _action);

            if (_action.Parent.HasValue)
            {
                foreach (var command in PropagateBackSetCompleted(_action.Parent.Value))
                {
                    yield return command;
                };
            }
        }

        public override IEnumerable<CommandEffect> BackSetCompleted()
        {
            throw new RunnerException("ActionScript shound't never completed from back!");
        }

        public override IEnumerable<CommandEffect> SetError()
        {
            _ = _control.FindCursor(_action.ActionId); // just to confirm cursor on it

            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Running
            }, $"Action in wrong status to Completed! {_action.ActionId}-{_action.Label}");

            _action.Status = ActionStatus.Error;
            yield return new CommandEffect(ComandEffectType.ActionUpdateStatus, _action);

            if (_action.Parent.HasValue)
            {
                foreach (var command in PropagateBackSetError(_action.Parent.Value))
                {
                    yield return command;
                };
            }
        }

        public override IEnumerable<CommandEffect> BackSetError()
        {
            throw new RunnerException("ActionScript shound't never error from back!");
        }

        public override IEnumerable<CommandEffect> Stop()
        {
            _ = _control.FindCursor(_action.ActionId); // just to confirm cursor on it

            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Running
            }, $"Action in wrong status to Completed! {_action.ActionId}-{_action.Label}");

            _action.Status = ActionStatus.ToStop;
            yield return new CommandEffect(ComandEffectType.ActionUpdateStatus, _action);

            if (_action.Parent.HasValue)
            {
                foreach (var command in PropagateBackStop(_action.Parent.Value))
                {
                    yield return command;
                };
            }
        }

        public override IEnumerable<CommandEffect> BackStop()
        {
            throw new RunnerException("ActionScript shound't never stop from back!");
        }

        public override IEnumerable<CommandEffect> SetStopped()
        {
            _ = _control.FindCursor(_action.ActionId); // just to confirm cursor on it

            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.ToStop
            }, $"Action in wrong status to Completed! {_action.ActionId}-{_action.Label}");

            _action.Status = ActionStatus.Stopped;
            yield return new CommandEffect(ComandEffectType.ActionUpdateStatus, _action);

            if (_action.Parent.HasValue)
            {
                foreach (var command in PropagateBackSetStoppped(_action.Parent.Value))
                {
                    yield return command;
                };
            }
        }

        public override IEnumerable<CommandEffect> BackSetStopped()
        {
            throw new RunnerException("ActionScript shound't never stopped from back!");
        }


        public override IEnumerable<CommandEffect> SetBreakPoint()
        {
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Waiting,
                ActionStatus.Stopped,
                ActionStatus.Error
            }, $"Action in wrong status to set breakpoint! {_action.ActionId}-{_action.Label}");

            _action.BreakPoint = true;
            yield return new CommandEffect(ComandEffectType.ActionUpdateBreakPoint, _action);
        }

        public override IEnumerable<CommandEffect> CleanBreakPoint()
        {
            _action.BreakPoint = false;
            yield return new CommandEffect(ComandEffectType.ActionUpdateBreakPoint, _action);
        }

        public override IEnumerable<CommandEffect> BackBreakPoint()
        {
            throw new RunnerException("ActionScript shound't never breakpoint from back!");
        }
    }
}
