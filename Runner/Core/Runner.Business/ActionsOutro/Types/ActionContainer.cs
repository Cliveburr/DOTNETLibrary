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
        public ActionContainer(Action action)
            : base(action)
        {
        }

        public override FowardRunResult FowardRun(CommandContext ctx)
        {
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Waiting
            }, $"ActionContainer in wrong status to Cursor! {_action.ActionId}-{_action.Label}");

            Assert.MustFalse(_action.WithCursor, $"ActionContainer already with Cursor! {_action.ActionId}-{_action.Label}");

            if (_action.BreakPoint)
            {
                _action.WithCursor = true;
                ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateWithCursor, _action));

                _action.Status = ActionStatus.Stopped;
                ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, _action));

                return FowardRunResult.WasBreakPoint;
            }
            else
            {
                Assert.MustNotNull(_action.Childs, $"ActionContainer with invalid Childs! {_action.ActionId}-{_action.Label}");

                if (_action.Childs.Any())
                {
                    var firstActionIdChild = _action.Childs.First();
                    var (nextAction, nextActionType) = ctx.Control.FindActionAndType(firstActionIdChild);

                    var childFowardRunResult = nextActionType.FowardRun(ctx);
                    switch (childFowardRunResult)
                    {
                        case FowardRunResult.WasBreakPoint:
                            {
                                _action.Status = ActionStatus.Stopped;
                                ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, _action));
                                return FowardRunResult.WasBreakPoint;
                            }
                        case FowardRunResult.WasCompleted:
                            {
                                _action.Status = ActionStatus.Completed;
                                ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, _action));
                                return FowardRunResult.WasCompleted;
                            }
                        case FowardRunResult.Running:
                            {
                                _action.Status = ActionStatus.Running;
                                ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, _action));
                                return FowardRunResult.Running;
                            }
                        default: throw new RunnerException($"ActionContainer receive invalid foward run result! {_action.ActionId}-{_action.Label}");
                    }
                }
                else
                {
                    _action.Status = ActionStatus.Completed;
                    ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, _action));

                    return FowardRunResult.WasCompleted;
                }
            }
        }

        public override void Run(CommandContext ctx)
        {
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Waiting,
                ActionStatus.Stopped,
                ActionStatus.Error
            }, $"ActionContainer in wrong status to Run! {_action.ActionId}-{_action.Label}");

            Assert.MustTrue(_action.WithCursor, $"ActionContainer in without cursor to Run! {_action.ActionId}-{_action.Label}");

            _action.WithCursor = false;
            ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateWithCursor, _action));

            Assert.MustNotNull(_action.Childs, $"ActionParallel with invalid Childs! {_action.ActionId}-{_action.Label}");

            if (_action.Childs.Any())
            {
                var firstActionIdChild = _action.Childs.First();
                var (nextAction, nextActionType) = ctx.Control.FindActionAndType(firstActionIdChild);

                ExecuteFowardRun(ctx, nextActionType);
            }
            else
            {
                _action.Status = ActionStatus.Completed;
                ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, _action));

                if (_action.Parent.HasValue)
                {
                    var (action, actionType) = ctx.Control.FindActionAndType(_action.Parent.Value);
                    actionType.BackSetCompleted(ctx, _action.ActionId);
                }
            }
        }

        private void ExecuteFowardRun(CommandContext ctx, ActionTypesBase nextActionType)
        {
            var childFowardRunResult = nextActionType.FowardRun(ctx);
            switch (childFowardRunResult)
            {
                case FowardRunResult.WasBreakPoint:
                    {
                        if (_action.Status != ActionStatus.Stopped)
                        {
                            _action.Status = ActionStatus.Stopped;
                            ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, _action));

                            if (_action.Parent.HasValue)
                            {
                                var (action, actionType) = ctx.Control.FindActionAndType(_action.Parent.Value);
                                actionType.BackBreakPoint(ctx);
                            }
                        }
                        break;
                    }
                case FowardRunResult.WasCompleted:
                    {
                        if (_action.Status != ActionStatus.Completed)
                        {
                            _action.Status = ActionStatus.Completed;
                            ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, _action));

                            if (_action.Parent.HasValue)
                            {
                                var (action, actionType) = ctx.Control.FindActionAndType(_action.Parent.Value);
                                actionType.BackSetCompleted(ctx, _action.ActionId);
                            }
                        }
                        break;
                    }
                case FowardRunResult.Running:
                    {
                        if (_action.Status != ActionStatus.Running)
                        {
                            _action.Status = ActionStatus.Running;
                            ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, _action));

                            if (_action.Parent.HasValue)
                            {
                                var (action, actionType) = ctx.Control.FindActionAndType(_action.Parent.Value);
                                actionType.BackRun(ctx);
                            }
                        }
                        break;
                    }
                default: throw new RunnerException($"ActionContainer receive invalid foward run result! {_action.ActionId}-{_action.Label}");
            }
        }

        public override void BackRun(CommandContext ctx)
        {
            if (_action.Status != ActionStatus.Running)
            {
                Assert.Enum.In(_action.Status, new[] {
                    ActionStatus.Stopped,
                    ActionStatus.Error
                }, $"ActionContainer container in wrong status to BackRun! {_action.ActionId}-{_action.Label}");

                _action.Status = ActionStatus.Running;
                ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, _action));
            }

            if (_action.Parent.HasValue)
            {
                var (action, actionType) = ctx.Control.FindActionAndType(_action.Parent.Value);
                actionType.BackRun(ctx);
            }
        }

        public override void SetRunning(CommandContext ctx)
        {
            throw new RunnerException("ActionContainer shound't never call running!");
        }

        public override void BackSetRunning(CommandContext ctx)
        {
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Running
            }, $"ActionContainer in wrong status to BackSetRunning! {_action.ActionId}-{_action.Label}");

            if (_action.Parent.HasValue)
            {
                var (action, actionType) = ctx.Control.FindActionAndType(_action.Parent.Value);
                actionType.BackSetRunning(ctx);
            }
        }

        public override void SetCompleted(CommandContext ctx)
        {
            throw new RunnerException("ActionContainer shound't never call completed!");
        }

        public override void BackSetCompleted(CommandContext ctx, int actionChildId)
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
                ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, _action));

                if (_action.Parent.HasValue)
                {
                    var (action, actionType) = ctx.Control.FindActionAndType(_action.Parent.Value);
                    actionType.BackSetCompleted(ctx, _action.ActionId);
                }
            }
            else
            {
                var nextActionId = _action.Childs[nextChildIndex];
                var (nextAction, nextActionType) = ctx.Control.FindActionAndType(nextActionId);

                ExecuteFowardRun(ctx, nextActionType);
            }
        }

        public override void SetError(CommandContext ctx)
        {
            throw new RunnerException("ActionContainer shound't never call error!");
        }

        public override void BackSetError(CommandContext ctx)
        {
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Running,
                ActionStatus.ToStop
            }, $"Action container in wrong status to BackSetError! {_action.ActionId}-{_action.Label}");

            _action.Status = ActionStatus.Error;
            ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, _action));

            if (_action.Parent.HasValue)
            {
                var (action, actionType) = ctx.Control.FindActionAndType(_action.Parent.Value);
                actionType.BackSetError(ctx);
            }
        }

        public override void Stop(CommandContext ctx)
        {
            throw new RunnerException("ActionContainer shound't never call stop!");
        }

        public override void BackStop(CommandContext ctx)
        {
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Running
            }, $"ActionContainer in wrong status to Stop! {_action.ActionId}-{_action.Label}");

            if (_action.Parent.HasValue)
            {
                var (action, actionType) = ctx.Control.FindActionAndType(_action.Parent.Value);
                actionType.BackStop(ctx);
            }
        }

        public override void SetStopped(CommandContext ctx)
        {
            throw new RunnerException("ActionContainer shound't never call stopped!");
        }

        public override void BackSetStopped(CommandContext ctx)
        {
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Running
            }, $"ActionContainer in wrong status to Stopped! {_action.ActionId}-{_action.Label}");

            _action.Status = ActionStatus.Stopped;
            ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, _action));

            if (_action.Parent.HasValue)
            {
                var (action, actionType) = ctx.Control.FindActionAndType(_action.Parent.Value);
                actionType.BackSetStopped(ctx);
            }
        }

        public override void SetBreakPoint(CommandContext ctx)
        {
            throw new RunnerException("ActionContainer shound't never call set breakpoint!");
        }

        public override void CleanBreakPoint(CommandContext ctx)
        {
            throw new RunnerException("ActionContainer shound't never call clean breakpoint!");
        }

        public override void BackBreakPoint(CommandContext ctx)
        {
            if (_action.Status != ActionStatus.Stopped)
            {
                _action.Status = ActionStatus.Stopped;
                ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, _action));
            }

            if (_action.Parent.HasValue)
            {
                var (action, actionType) = ctx.Control.FindActionAndType(_action.Parent.Value);
                actionType.BackBreakPoint(ctx);
            }
        }
    }
}
