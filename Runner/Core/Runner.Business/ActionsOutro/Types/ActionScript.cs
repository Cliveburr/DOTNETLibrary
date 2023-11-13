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

        public override IEnumerable<CommandEffect> ContinueRun()
        {
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Waiting
            }, $"ActionScript in wrong status to Cursor! {_action.ActionId}-{_action.Label}");

            Assert.MustFalse(_action.WithCursor, $"ActionScript already with Cursor! {_action.ActionId}-{_action.Label}");

            _action.WithCursor = true;
            yield return new CommandEffect(ComandEffectType.ActionUpdateWithCursor, _action);

            if (_action.BreakPoint)
            {
                _action.Status = ActionStatus.Stopped;
                yield return new CommandEffect(ComandEffectType.ActionUpdateStatus, _action);

                foreach (var command in PropagateBackBreakPoint())
                {
                    yield return command;
                };
            }
            else
            {
                foreach (var command in Run())
                {
                    yield return command;
                }
            }
        }

        public override IEnumerable<CommandEffect> Run()
        {
            Assert.MustTrue(_action.WithCursor, $"ActionScript in without cursor to Run! {_action.ActionId}-{_action.Label}");

            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Waiting,
                ActionStatus.Stopped,
                ActionStatus.Error
            }, $"ActionScript in wrong status to Run! {_action.ActionId}-{_action.Label}");

            _action.Status = ActionStatus.ToRun;
            yield return new CommandEffect(ComandEffectType.ActionUpdateToRun, _action);

            foreach (var command in PropagateBackRun())
            {
                yield return command;
            };
        }

        public override IEnumerable<CommandEffect> BackRun()
        {
            throw new RunnerException("ActionScript shound't never run from back!");
        }

        public override IEnumerable<CommandEffect> SetRunning()
        {
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.ToRun
            }, $"ActionScript in wrong status to Running! {_action.ActionId}-{_action.Label}");

            _action.Status = ActionStatus.Running;
            yield return new CommandEffect(ComandEffectType.ActionUpdateStatus, _action);

            foreach (var command in PropagateBackSetRunning())
            {
                yield return command;
            };
        }

        public override IEnumerable<CommandEffect> BackSetRunning()
        {
            throw new RunnerException("ActionScript shound't never running from back!");
        }

        public override IEnumerable<CommandEffect> SetCompleted()
        {
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Running,
                ActionStatus.ToStop
            }, $"ActionScript in wrong status to Completed! {_action.ActionId}-{_action.Label}");

            _action.Status = ActionStatus.Completed;
            yield return new CommandEffect(ComandEffectType.ActionUpdateStatus, _action);

            _action.WithCursor = false;
            yield return new CommandEffect(ComandEffectType.ActionUpdateWithCursor, _action);

            foreach (var command in PropagateBackSetCompleted())
            {
                yield return command;
            };
        }

        public override IEnumerable<CommandEffect> BackSetCompleted(int actionChildId)
        {
            throw new RunnerException("ActionScript shound't never completed from back!");
        }

        public override IEnumerable<CommandEffect> SetError()
        {
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Running
            }, $"ActionScript in wrong status to Completed! {_action.ActionId}-{_action.Label}");

            _action.Status = ActionStatus.Error;
            yield return new CommandEffect(ComandEffectType.ActionUpdateStatus, _action);

            foreach (var command in PropagateBackSetError())
            {
                yield return command;
            };
        }

        public override IEnumerable<CommandEffect> BackSetError()
        {
            throw new RunnerException("ActionScript shound't never error from back!");
        }

        public override IEnumerable<CommandEffect> Stop()
        {
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Running
            }, $"ActionScript in wrong status to Completed! {_action.ActionId}-{_action.Label}");

            _action.Status = ActionStatus.ToStop;
            yield return new CommandEffect(ComandEffectType.ActionUpdateStatus, _action);

            foreach (var command in PropagateBackStop())
            {
                yield return command;
            };
        }

        public override IEnumerable<CommandEffect> BackStop()
        {
            throw new RunnerException("ActionScript shound't never stop from back!");
        }

        public override IEnumerable<CommandEffect> SetStopped()
        {
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.ToStop
            }, $"ActionScript in wrong status to Completed! {_action.ActionId}-{_action.Label}");

            _action.Status = ActionStatus.Stopped;
            yield return new CommandEffect(ComandEffectType.ActionUpdateStatus, _action);

            foreach (var command in PropagateBackSetStoppped())
            {
                yield return command;
            };
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
            }, $"ActionScript in wrong status to set breakpoint! {_action.ActionId}-{_action.Label}");

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
