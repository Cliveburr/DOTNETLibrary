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

        public override IEnumerable<CommandEffect> Run()
        {
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Waiting,
                ActionStatus.Stopped,
                ActionStatus.Error
            }, $"Action in wrong status to Run! {_action.ActionId}-{_action.Label}");

            var cursorHasPassedOverMe = _control.FindCursorOnPasseds(_action.ActionId)
                .FirstOrDefault();

            if (cursorHasPassedOverMe is null)
            {
                var cursor = _control.FindCursor(_action.ActionId);

                if (_action.Childs is not null && _action.Childs.Any())
                {
                    var firstActionIdChild = _action.Childs.First();

                    foreach (var command in MoveCursorFoward(cursor, firstActionIdChild))
                    {
                        yield return command;
                    }

                    var nextAction = _control.FindAction(firstActionIdChild);
                    if (nextAction.BreakPoint)
                    {
                        _action.Status = ActionStatus.Stopped;
                        yield return new CommandEffect(ComandEffectType.ActionUpdateStatus, _action);

                        if (_action.Parent.HasValue)
                        {
                            foreach (var command in PropagateBackBreakPoint(_action.Parent.Value))
                            {
                                yield return command;
                            }
                        }
                        yield break;
                    }
                    else
                    {
                        _action.Status = ActionStatus.Running;
                        yield return new CommandEffect(ComandEffectType.ActionUpdateStatus, _action);

                        foreach (var command in _control.Run(firstActionIdChild))
                        {
                            yield return command;
                        }

                        if (_action.Parent.HasValue)
                        {
                            foreach (var command in PropagateBackRun(_action.Parent.Value))
                            {
                                yield return command;
                            }
                        }
                    }
                }
                else
                {
                    _action.Status = ActionStatus.Completed;
                    yield return new CommandEffect(ComandEffectType.ActionUpdateStatus, _action);

                    if (_action.Parent.HasValue)
                    {
                        foreach (var command in PropagateBackSetCompleted(_action.Parent.Value))
                        {
                            yield return command;
                        }
                    }
                }
            }
            else
            {
                var actionIdOnCursor = cursorHasPassedOverMe.ActionId;
                var currentAction = _control.FindAction(actionIdOnCursor);

                if (currentAction.BreakPoint)
                {
                    _action.Status = ActionStatus.Stopped;
                    yield return new CommandEffect(ComandEffectType.ActionUpdateStatus, _action);

                    if (_action.Parent.HasValue)
                    {
                        foreach (var command in PropagateBackBreakPoint(_action.Parent.Value))
                        {
                            yield return command;
                        }
                    }
                    yield break;
                }
                else
                {
                    _action.Status = ActionStatus.Running;
                    yield return new CommandEffect(ComandEffectType.ActionUpdateStatus, _action);

                    foreach (var command in _control.Run(currentAction.ActionId))
                    {
                        yield return command;
                    }

                    if (_action.Parent.HasValue)
                    {
                        foreach (var command in PropagateBackRun(_action.Parent.Value))
                        {
                            yield return command;
                        }
                    }
                }
            }
        }

        public override IEnumerable<CommandEffect> BackRun()
        {
            if (_action.Status == ActionStatus.Running)
            {
                yield break;
            }

            var (myCursor, currentActionOnMyCursor) = _control.FindCurrentActionOnMyCursor(_action.ActionId);

            Assert.Enum.In(currentActionOnMyCursor.Status, new[] {
                ActionStatus.ToRun
            }, $"Action child in wrong status to Run! {_action.ActionId}-{_action.Label}");

            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Waiting,
                ActionStatus.Stopped,
                ActionStatus.Error
            }, $"Action container in wrong status to Run! {_action.ActionId}-{_action.Label}");

            _action.Status = ActionStatus.Running;
            yield return new CommandEffect(ComandEffectType.ActionUpdateStatus, _action);

            if (_action.Parent.HasValue)
            {
                foreach (var command in PropagateBackRun(_action.Parent.Value))
                {
                    yield return command;
                }
            }
        }

        public override IEnumerable<CommandEffect> SetRunning()
        {
            throw new RunnerException("ActionContainer shound't never call running!");
        }

        public override IEnumerable<CommandEffect> BackSetRunning()
        {
            var (myCursor, currentActionOnMyCursor) = _control.FindCurrentActionOnMyCursor(_action.ActionId);

            Assert.Enum.In(currentActionOnMyCursor.Status, new[] {
                ActionStatus.Running
            }, $"Action child in wrong status to BackSetRunning! {_action.ActionId}-{_action.Label}");

            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Running
            }, $"Action container in wrong status to BackSetRunning! {_action.ActionId}-{_action.Label}");

            yield break;
        }

        public override IEnumerable<CommandEffect> SetCompleted()
        {
            throw new RunnerException("ActionContainer shound't never call completed!");
        }

        public override IEnumerable<CommandEffect> BackSetCompleted()
        {
            var (myCursor, currentActionOnMyCursor) = _control.FindCurrentActionOnMyCursor(_action.ActionId);

            Assert.Enum.In(currentActionOnMyCursor.Status, new[] {
                ActionStatus.Completed
            }, $"Action child in wrong status to BackSetCompleted! {_action.ActionId}-{_action.Label}");

            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Running
            }, $"Action container in wrong status to BackSetCompleted! {_action.ActionId}-{_action.Label}");

            Assert.MustNotNull(_action.Childs, $"Action container with invalid Childs! {_action.ActionId}-{_action.Label}");

            var currentChildIndex = _action.Childs.IndexOf(currentActionOnMyCursor.ActionId);
            Assert.Number.InRange(currentChildIndex, 0, _action.Childs.Count - 1, $"Invalid child index in parent! {_action.ActionId}-{_action.Label}");

            var nextChildIndex = currentChildIndex + 1;
            if (nextChildIndex == _action.Childs.Count)
            {
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
            else
            {
                var nextActionId = _action.Childs[nextChildIndex];
                foreach (var command in MoveCursorFoward(myCursor, nextActionId))
                {
                    yield return command;
                }

                var nextAction = _control.FindAction(nextActionId);
                if (nextAction.BreakPoint)
                {
                    _action.Status = ActionStatus.Stopped;
                    yield return new CommandEffect(ComandEffectType.ActionUpdateStatus, _action);

                    if (_action.Parent.HasValue)
                    {
                        foreach (var command in PropagateBackBreakPoint(_action.Parent.Value))
                        {
                            yield return command;
                        }
                    }
                }   
                else
                {
                    foreach (var command in _control.Run(nextActionId))
                    {
                        yield return command;
                    }
                }
            }
        }

        public override IEnumerable<CommandEffect> SetError()
        {
            throw new RunnerException("ActionContainer shound't never call error!");
        }

        public override IEnumerable<CommandEffect> BackSetError()
        {
            var (myCursor, currentActionOnMyCursor) = _control.FindCurrentActionOnMyCursor(_action.ActionId);

            Assert.Enum.In(currentActionOnMyCursor.Status, new[] {
                ActionStatus.Error
            }, $"Action child in wrong status to BackSetError! {_action.ActionId}-{_action.Label}");

            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Running
            }, $"Action container in wrong status to BackSetError! {_action.ActionId}-{_action.Label}");

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

        public override IEnumerable<CommandEffect> Stop()
        {
            var (myCursor, currentActionOnMyCursor) = _control.FindCurrentActionOnMyCursor(_action.ActionId);

            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Running
            }, $"ActionContainer in wrong status to Stop! {_action.ActionId}-{_action.Label}");

            foreach (var command in _control.Stop(currentActionOnMyCursor.ActionId))
            {
                yield return command;
            };
        }

        public override IEnumerable<CommandEffect> BackStop()
        {
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Running
            }, $"ActionContainer in wrong status to Stop! {_action.ActionId}-{_action.Label}");

            if (_action.Parent.HasValue)
            {
                foreach (var command in PropagateBackStop(_action.Parent.Value))
                {
                    yield return command;
                };
            }
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

            if (_action.Parent.HasValue)
            {
                foreach (var command in PropagateBackSetStoppped(_action.Parent.Value))
                {
                    yield return command;
                };
            }
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
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Waiting,
                ActionStatus.Stopped,
                ActionStatus.Error,
                ActionStatus.Running
            }, $"ActionContainer in wrong status to Stopped! {_action.ActionId}-{_action.Label}");

            _action.Status = ActionStatus.Stopped;
            yield return new CommandEffect(ComandEffectType.ActionUpdateStatus, _action);

            if (_action.Parent.HasValue)
            {
                foreach (var command in PropagateBackBreakPoint(_action.Parent.Value))
                {
                    yield return command;
                };
            }
        }
    }
}
