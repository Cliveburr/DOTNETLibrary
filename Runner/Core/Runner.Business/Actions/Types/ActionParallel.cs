using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Actions.Types
{
    public class ActionParallel : ActionTypesBase
    {
        public ActionParallel(Action action)
            : base(action)
        {
        }

        public override FowardRunResult FowardRun(CommandContext ctx)
        {
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Waiting
            }, $"ActionParallel in wrong status to Cursor! {_action.ActionId}-{_action.Label}");

            Assert.MustFalse(_action.WithCursor, $"ActionParallel already with Cursor! {_action.ActionId}-{_action.Label}");

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
                if (_action.Childs is not null && _action.Childs.Any())
                {
                    var fowardRunResults = new List<FowardRunResult>();
                    foreach (var childAction in _action.Childs)
                    {
                        var (nextAction, nextActionType) = ctx.Control.FindActionAndType(childAction);
                        fowardRunResults.Add(nextActionType.FowardRun(ctx));
                    }

                    var isSomeoneRunning = fowardRunResults
                        .Any(cs => cs == FowardRunResult.Running);
                    if (isSomeoneRunning)
                    {
                        _action.Status = ActionStatus.Running;
                        ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, _action));
                        return FowardRunResult.Running;
                    }

                    var isSomeoneBreakPoint = fowardRunResults
                        .Any(cs => cs == FowardRunResult.WasBreakPoint);
                    if (isSomeoneBreakPoint)
                    {
                        _action.Status = ActionStatus.Stopped;
                        ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, _action));
                        return FowardRunResult.WasBreakPoint;
                    }

                    return FowardRunResult.WasCompleted;
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
            }, $"ActionParallel in wrong status to Run! {_action.ActionId}-{_action.Label}");

            Assert.MustTrue(_action.WithCursor, $"ActionParallel in without cursor to Run! {_action.ActionId}-{_action.Label}");

            _action.WithCursor = false;
            ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateWithCursor, _action));

            if (_action.Childs is not null && _action.Childs.Any())
            {
                foreach (var childAction in _action.Childs)
                {
                    var (nextAction, nextActionType) = ctx.Control.FindActionAndType(childAction);
                    _ = nextActionType.FowardRun(ctx);
                }

                CheckStatus(ctx);
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

        public override void BackRun(CommandContext ctx)
        {
            if (_action.Status != ActionStatus.Running)
            {
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
            throw new RunnerException("ActionParallel shound't never call running!");
        }

        public override void BackSetRunning(CommandContext ctx)
        {
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Running
            }, $"ActionParallel in wrong status to BackSetRunning! {_action.ActionId}-{_action.Label}");

            CheckStatus(ctx);
        }

        public override void SetCompleted(CommandContext ctx)
        {
            throw new RunnerException("ActionParallel shound't never call completed!");
        }

        public override void BackSetCompleted(CommandContext ctx, int actionChildId)
        {
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Running,
                ActionStatus.ToStop
            }, $"ActionParallel in wrong status to BackSetCompleted! {_action.ActionId}-{_action.Label}");

            CheckStatus(ctx);
        }

        public override void SetError(CommandContext ctx)
        {
            throw new RunnerException("ActionParallel shound't never call error!");
        }

        public override void BackSetError(CommandContext ctx)
        {
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Running,
                ActionStatus.ToStop
            }, $"Action container in wrong status to BackSetError! {_action.ActionId}-{_action.Label}");

            CheckStatus(ctx);
        }


        public override void Stop(CommandContext ctx)
        {
            throw new RunnerException("ActionParallel shound't never call stop!");
        }

        public override void BackStop(CommandContext ctx)
        {
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Running
            }, $"ActionParallel in wrong status to Stop! {_action.ActionId}-{_action.Label}");

            if (_action.Parent.HasValue)
            {
                var (action, actionType) = ctx.Control.FindActionAndType(_action.Parent.Value);
                actionType.BackStop(ctx);
            }
        }

        public override void SetStopped(CommandContext ctx)
        {
            throw new RunnerException("ActionParallel shound't never call stopped!");
        }

        public override void BackSetStopped(CommandContext ctx)
        {
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Running
            }, $"ActionParallel in wrong status to Stopped! {_action.ActionId}-{_action.Label}");

            CheckStatus(ctx);
        }

        public override void SetBreakPoint(CommandContext ctx)
        {
            throw new RunnerException("ActionParallel shound't never call set breakpoint!");
        }

        public override void CleanBreakPoint(CommandContext ctx)
        {
            throw new RunnerException("ActionParallel shound't never call clean breakpoint!");
        }

        public override void BackBreakPoint(CommandContext ctx)
        {
            CheckStatus(ctx);

            if (_action.Parent.HasValue)
            {
                var (action, actionType) = ctx.Control.FindActionAndType(_action.Parent.Value);
                actionType.BackBreakPoint(ctx);
            }
        }

        private void CheckStatus(CommandContext ctx)
        {
            /*
                Waiting = 0,
                ToRun =  isSomeoneRunning
                Running = isSomeoneRunning
                ToStop = isSomeoneRunning
                Stopped = isSomeoneStopped
                Error = isSomeoneError
                Completed = 6
            */

            Assert.MustNotNull(_action.Childs, $"ActionParallel with invalid Childs! {_action.ActionId}-{_action.Label}");

            var childsStatus = _action.Childs
                .Select(i => ctx.Control.FindAction(i).Status)
                .ToList();

            var isSomeoneRunning = childsStatus
                .Any(cs => cs == ActionStatus.Running ||
                    cs == ActionStatus.ToStop ||
                    cs == ActionStatus.ToRun);
            if (isSomeoneRunning)
            {
                if (_action.Status != ActionStatus.Running)
                {
                    _action.Status = ActionStatus.Running;
                    ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, _action));

                    if (_action.Parent.HasValue)
                    {
                        var (action, actionType) = ctx.Control.FindActionAndType(_action.Parent.Value);
                        actionType.BackSetRunning(ctx);
                    }
                }
                return;
            }

            var isSomeoneError = childsStatus
                .Any(cs => cs == ActionStatus.Error);
            if (isSomeoneError)
            {
                if (_action.Status != ActionStatus.Error)
                {
                    _action.Status = ActionStatus.Error;
                    ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, _action));

                    if (_action.Parent.HasValue)
                    {
                        var (action, actionType) = ctx.Control.FindActionAndType(_action.Parent.Value);
                        actionType.BackSetError(ctx);
                    }
                }
                return;
            }

            var isSomeoneStopped = childsStatus
                .Any(cs => cs == ActionStatus.Stopped);
            if (isSomeoneStopped)
            {
                if (_action.Status != ActionStatus.Stopped)
                {
                    _action.Status = ActionStatus.Stopped;
                    ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, _action));

                    if (_action.Parent.HasValue)
                    {
                        var (action, actionType) = ctx.Control.FindActionAndType(_action.Parent.Value);
                        actionType.BackSetStopped(ctx);
                    }

                }
                return;
            }

            var isAllCompleted = !childsStatus
                .Any(ac => ac != ActionStatus.Completed);
            if (isAllCompleted)
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
                return;
            }
        }
    }
}
