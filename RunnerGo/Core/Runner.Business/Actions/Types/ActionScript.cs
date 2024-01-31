
namespace Runner.Business.Actions.Types
{
    public class ActionScript : ActionTypesBase
    {
        public ActionScript(Action action)
            : base(action)
        {
        }

        public override FowardRunResult FowardRun(CommandContext ctx)
        {
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Waiting
            }, $"ActionScript in wrong status to Cursor! {_action.ActionId}-{_action.Label}");

            Assert.MustFalse(_action.WithCursor, $"ActionScript already with Cursor! {_action.ActionId}-{_action.Label}");

            _action.WithCursor = true;
            ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateWithCursor, _action));

            if (_action.BreakPoint)
            {
                _action.Status = ActionStatus.Stopped;
                ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, _action));

                return FowardRunResult.WasBreakPoint;
            }
            else
            {
                _action.Status = ActionStatus.ToRun;
                ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateToRun, _action));

                return FowardRunResult.Running;
            }
        }

        public override void Run(CommandContext ctx)
        {
            Assert.MustTrue(_action.WithCursor, $"ActionScript in without cursor to Run! {_action.ActionId}-{_action.Label}");

            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Waiting,
                ActionStatus.Stopped,
                ActionStatus.Error
            }, $"ActionScript in wrong status to Run! {_action.ActionId}-{_action.Label}");

            _action.Status = ActionStatus.ToRun;
            ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateToRun, _action));

            if (_action.Parent.HasValue)
            {
                var (action, actionType) = ctx.Control.FindActionAndType(_action.Parent.Value);
                actionType.BackRun(ctx);
            }
        }

        public override void BackRun(CommandContext ctx)
        {
            throw new RunnerException("ActionScript shound't never run from back!");
        }

        public override void SetRunning(CommandContext ctx)
        {
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.ToRun
            }, $"ActionScript in wrong status to Running! {_action.ActionId}-{_action.Label}");

            _action.Status = ActionStatus.Running;
            ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, _action));

            if (_action.Parent.HasValue)
            {
                var (action, actionType) = ctx.Control.FindActionAndType(_action.Parent.Value);
                actionType.BackSetRunning(ctx);
            }
        }

        public override void BackSetRunning(CommandContext ctx)
        {
            throw new RunnerException("ActionScript shound't never running from back!");
        }

        public override void SetCompleted(CommandContext ctx)
        {
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Running,
                ActionStatus.ToStop
            }, $"ActionScript in wrong status to Completed! {_action.ActionId}-{_action.Label}");

            _action.Status = ActionStatus.Completed;
            ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, _action));

            _action.WithCursor = false;
            ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateWithCursor, _action));

            if (_action.Parent.HasValue)
            {
                var (action, actionType) = ctx.Control.FindActionAndType(_action.Parent.Value);
                actionType.BackSetCompleted(ctx, _action.ActionId);
            }
        }

        public override void BackSetCompleted(CommandContext ctx, int actionChildId)
        {
            throw new RunnerException("ActionScript shound't never completed from back!");
        }

        public override void SetError(CommandContext ctx)
        {
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Running
            }, $"ActionScript in wrong status to Completed! {_action.ActionId}-{_action.Label}");

            _action.Status = ActionStatus.Error;
            ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, _action));

            if (_action.Parent.HasValue)
            {
                var (action, actionType) = ctx.Control.FindActionAndType(_action.Parent.Value);
                actionType.BackSetError(ctx);
            }
        }

        public override void BackSetError(CommandContext ctx)
        {
            throw new RunnerException("ActionScript shound't never error from back!");
        }

        public override void Stop(CommandContext ctx)
        {
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Running,
                ActionStatus.ToRun
            }, $"ActionScript in wrong status to Completed! {_action.ActionId}-{_action.Label}");

            if (_action.Status == ActionStatus.Running)
            {
                _action.Status = ActionStatus.ToStop;
                ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateToStop, _action));

                if (_action.Parent.HasValue)
                {
                    var (action, actionType) = ctx.Control.FindActionAndType(_action.Parent.Value);
                    actionType.BackStop(ctx);
                }
            }
            else
            {
                _action.Status = ActionStatus.Stopped;
                ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateToStop, _action));

                if (_action.Parent.HasValue)
                {
                    var (action, actionType) = ctx.Control.FindActionAndType(_action.Parent.Value);
                    actionType.BackSetStopped(ctx);
                }
            }
        }

        public override void BackStop(CommandContext ctx)
        {
            throw new RunnerException("ActionScript shound't never stop from back!");
        }

        public override void SetStopped(CommandContext ctx)
        {
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.ToStop
            }, $"ActionScript in wrong status to Completed! {_action.ActionId}-{_action.Label}");

            _action.Status = ActionStatus.Stopped;
            ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, _action));

            if (_action.Parent.HasValue)
            {
                var (action, actionType) = ctx.Control.FindActionAndType(_action.Parent.Value);
                actionType.BackSetStopped(ctx);
            }
        }

        public override void BackSetStopped(CommandContext ctx)
        {
            throw new RunnerException("ActionScript shound't never stopped from back!");
        }

        public override void SetBreakPoint(CommandContext ctx)
        {
            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Waiting,
                ActionStatus.Stopped,
                ActionStatus.Error
            }, $"ActionScript in wrong status to set breakpoint! {_action.ActionId}-{_action.Label}");

            _action.BreakPoint = true;
            ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateBreakPoint, _action));
        }

        public override void CleanBreakPoint(CommandContext ctx)
        {
            _action.BreakPoint = false;
            ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateBreakPoint, _action));
        }

        public override void BackBreakPoint(CommandContext ctx)
        {
            throw new RunnerException("ActionScript shound't never breakpoint from back!");
        }
    }
}
