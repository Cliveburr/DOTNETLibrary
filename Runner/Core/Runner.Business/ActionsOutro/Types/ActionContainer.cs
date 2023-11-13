using Microsoft.VisualBasic;
using Runner.Business.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.ActionsOutro.Types
{
    public class ActionContainer : ActionTypesBase
    {
        public ActionContainer(ActionControl control, Action action)
            : base(control, action)
        {
        }

        public override IEnumerable<CommandEffect> ContinueRun()
        {
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Waiting
            }, $"ActionContainer in wrong status to Cursor! {_action.ActionId}-{_action.Label}");

            Assert.MustFalse(_action.WithCursor, $"ActionContainer already with Cursor! {_action.ActionId}-{_action.Label}");

            if (_action.BreakPoint)
            {
                _action.WithCursor = true;
                yield return new CommandEffect(ComandEffectType.ActionUpdateWithCursor, _action);

                _action.Status = ActionStatus.Stopped;
                yield return new CommandEffect(ComandEffectType.ActionUpdateStatus, _action);

                foreach (var command in PropagateBackBreakPoint())
                {
                    yield return command;
                };
            }
            else
            {
                foreach (var command in InterRun())
                {
                    yield return command;
                }
            }
        }

        public override IEnumerable<CommandEffect> Run()
        {
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Waiting,
                ActionStatus.Stopped,
                ActionStatus.Error
            }, $"ActionContainer in wrong status to Run! {_action.ActionId}-{_action.Label}");

            Assert.MustTrue(_action.WithCursor, $"ActionContainer in without cursor to Run! {_action.ActionId}-{_action.Label}");

            _action.WithCursor = false;
            yield return new CommandEffect(ComandEffectType.ActionUpdateWithCursor, _action);

            foreach (var command in InterRun())
            {
                yield return command;
            }
        }

        private IEnumerable<CommandEffect> InterRun()
        {
            if (_action.Childs is not null && _action.Childs.Any())
            {
                _action.Status = ActionStatus.Running;

                var firstActionIdChild = _action.Childs.First();
                var (nextAction, nextActionType) = _control.FindActionAndType(firstActionIdChild);

                foreach (var command in nextActionType.ContinueRun())
                {
                    yield return command;
                }

                if (_action.Status == ActionStatus.Running)
                {
                    yield return new CommandEffect(ComandEffectType.ActionUpdateStatus, _action);

                    foreach (var command in PropagateBackRun())
                    {
                        yield return command;
                    }
                }
            }
            else
            {
                _action.Status = ActionStatus.Completed;
                yield return new CommandEffect(ComandEffectType.ActionUpdateStatus, _action);

                foreach (var command in PropagateBackSetCompleted())
                {
                    yield return command;
                }
            }
        }

        public override IEnumerable<CommandEffect> BackRun()
        {
            if (_action.Status != ActionStatus.Running)
            {
                Assert.Enum.In(_action.Status, new[] {
                    ActionStatus.Stopped,
                    ActionStatus.Error
                }, $"ActionContainer container in wrong status to BackRun! {_action.ActionId}-{_action.Label}");

                _action.Status = ActionStatus.Running;
                yield return new CommandEffect(ComandEffectType.ActionUpdateStatus, _action);
            }

            foreach (var command in PropagateBackRun())
            {
                yield return command;
            }
        }

        public override IEnumerable<CommandEffect> SetRunning()
        {
            throw new RunnerException("ActionContainer shound't never call running!");
        }

        public override IEnumerable<CommandEffect> BackSetRunning()
        {
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Running
            }, $"ActionContainer in wrong status to BackSetRunning! {_action.ActionId}-{_action.Label}");

            foreach (var command in PropagateBackSetRunning())
            {
                yield return command;
            };
        }

        public override IEnumerable<CommandEffect> SetCompleted()
        {
            throw new RunnerException("ActionContainer shound't never call completed!");
        }

        public override IEnumerable<CommandEffect> BackSetCompleted(int actionChildId)
        {
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Running,
                ActionStatus.ToStop
            }, $"ActionContainer in wrong status to BackSetCompleted! {_action.ActionId}-{_action.Label}");

            Assert.MustNotNull(_action.Childs, $"Action container with invalid Childs! {_action.ActionId}-{_action.Label}");

            var currentChildIndex = _action.Childs.IndexOf(actionChildId);
            Assert.Number.InRange(currentChildIndex, 0, _action.Childs.Count - 1, $"Invalid child index in parent! {_action.ActionId}-{_action.Label}, ActionChildId: {actionChildId}");

            var nextChildIndex = currentChildIndex + 1;
            if (nextChildIndex == _action.Childs.Count)
            {
                _action.Status = ActionStatus.Completed;
                yield return new CommandEffect(ComandEffectType.ActionUpdateStatus, _action);

                foreach (var command in PropagateBackSetCompleted())
                {
                    yield return command;
                };
            }
            else
            {
                var nextActionId = _action.Childs[nextChildIndex];
                var (nextAction, nextActionType) = _control.FindActionAndType(nextActionId);

                foreach (var command in nextActionType.ContinueRun())
                {
                    yield return command;
                }
            }
        }

        public override IEnumerable<CommandEffect> SetError()
        {
            throw new RunnerException("ActionContainer shound't never call error!");
        }

        public override IEnumerable<CommandEffect> BackSetError()
        {
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Running
            }, $"Action container in wrong status to BackSetError! {_action.ActionId}-{_action.Label}");

            _action.Status = ActionStatus.Error;
            yield return new CommandEffect(ComandEffectType.ActionUpdateStatus, _action);

            foreach (var command in PropagateBackSetError())
            {
                yield return command;
            };
        }

        public override IEnumerable<CommandEffect> Stop()
        {
            throw new RunnerException("ActionContainer shound't never call stop!");
        }

        public override IEnumerable<CommandEffect> BackStop()
        {
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Running
            }, $"ActionContainer in wrong status to Stop! {_action.ActionId}-{_action.Label}");

            foreach (var command in PropagateBackStop())
            {
                yield return command;
            };
        }

        public override IEnumerable<CommandEffect> SetStopped()
        {
            throw new RunnerException("ActionContainer shound't never call stopped!");
        }

        public override IEnumerable<CommandEffect> BackSetStopped()
        {
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Running
            }, $"ActionContainer in wrong status to Stopped! {_action.ActionId}-{_action.Label}");

            _action.Status = ActionStatus.Stopped;
            yield return new CommandEffect(ComandEffectType.ActionUpdateStatus, _action);

            foreach (var command in PropagateBackSetStoppped())
            {
                yield return command;
            };
        }

        public override IEnumerable<CommandEffect> SetBreakPoint()
        {
            throw new RunnerException("ActionContainer shound't never call set breakpoint!");
        }

        public override IEnumerable<CommandEffect> CleanBreakPoint()
        {
            throw new RunnerException("ActionContainer shound't never call clean breakpoint!");
        }

        public override IEnumerable<CommandEffect> BackBreakPoint()
        {
            if (_action.Status != ActionStatus.Stopped)
            {
                _action.Status = ActionStatus.Stopped;
                yield return new CommandEffect(ComandEffectType.ActionUpdateStatus, _action);
            }

            foreach (var command in PropagateBackBreakPoint())
            {
                yield return command;
            };
        }
    }
}
