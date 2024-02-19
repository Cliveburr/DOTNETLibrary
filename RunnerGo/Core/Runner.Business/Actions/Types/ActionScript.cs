
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
            Assert.Enum.In(Action.Status, new[] {
                ActionStatus.Waiting
            }, $"ActionScript in wrong status to Cursor! {Action.ActionId}-{Action.Label}");

            Assert.MustFalse(Action.WithCursor, $"ActionScript already with Cursor! {Action.ActionId}-{Action.Label}");

            Action.WithCursor = true;
            ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateWithCursor, Action));

            if (Action.BreakPoint)
            {
                Action.Status = ActionStatus.Stopped;
                ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, Action));

                return FowardRunResult.WasBreakPoint;
            }
            else
            {
                Action.Status = ActionStatus.ToRun;
                ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateToRun, Action));

                return FowardRunResult.Running;
            }
        }

        public override void Run(CommandContext ctx)
        {
            Assert.MustTrue(Action.WithCursor, $"ActionScript in without cursor to Run! {Action.ActionId}-{Action.Label}");

            Assert.Enum.In(Action.Status, new[] {
                ActionStatus.Waiting,
                ActionStatus.Stopped,
                ActionStatus.Error
            }, $"ActionScript in wrong status to Run! {Action.ActionId}-{Action.Label}");

            Action.Status = ActionStatus.ToRun;
            ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateToRun, Action));

            if (Action.Parent.HasValue)
            {
                var (action, actionType) = ctx.Control.FindActionAndType(Action.Parent.Value);
                actionType.BackRun(ctx);
            }
        }

        public override void BackRun(CommandContext ctx)
        {
            throw new RunnerException("ActionScript shound't never run from back!");
        }

        public override void SetRunning(CommandContext ctx)
        {
            Assert.Enum.In(Action.Status, new[] {
                ActionStatus.ToRun
            }, $"ActionScript in wrong status to Running! {Action.ActionId}-{Action.Label}");

            Action.Status = ActionStatus.Running;
            ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, Action));

            if (Action.Parent.HasValue)
            {
                var (action, actionType) = ctx.Control.FindActionAndType(Action.Parent.Value);
                actionType.BackSetRunning(ctx);
            }
        }

        public override void BackSetRunning(CommandContext ctx)
        {
            throw new RunnerException("ActionScript shound't never running from back!");
        }

        public override void SetCompleted(CommandContext ctx)
        {
            Assert.Enum.In(Action.Status, new[] {
                ActionStatus.Running,
                ActionStatus.ToStop
            }, $"ActionScript in wrong status to Completed! {Action.ActionId}-{Action.Label}");

            Action.Status = ActionStatus.Completed;
            ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, Action));

            Action.WithCursor = false;
            ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateWithCursor, Action));

            if (Action.Parent.HasValue)
            {
                var (action, actionType) = ctx.Control.FindActionAndType(Action.Parent.Value);
                actionType.BackSetCompleted(ctx, Action.ActionId);
            }
        }

        public override void BackSetCompleted(CommandContext ctx, int actionChildId)
        {
            throw new RunnerException("ActionScript shound't never completed from back!");
        }

        public override void SetError(CommandContext ctx)
        {
            Assert.Enum.In(Action.Status, new[] {
                ActionStatus.Running,
                ActionStatus.ToRun,
                ActionStatus.ToRun
            }, $"ActionScript in wrong status to Error! {Action.ActionId}-{Action.Label}");

            Action.Status = ActionStatus.Error;
            ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, Action));

            if (Action.Parent.HasValue)
            {
                var (action, actionType) = ctx.Control.FindActionAndType(Action.Parent.Value);
                actionType.BackSetError(ctx);
            }
        }

        public override void BackSetError(CommandContext ctx)
        {
            throw new RunnerException("ActionScript shound't never error from back!");
        }

        public override void Stop(CommandContext ctx)
        {
            Assert.Enum.In(Action.Status, new[] {
                ActionStatus.Running,
                ActionStatus.ToRun
            }, $"ActionScript in wrong status to Completed! {Action.ActionId}-{Action.Label}");

            if (Action.Status == ActionStatus.Running)
            {
                Action.Status = ActionStatus.ToStop;
                ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateToStop, Action));

                if (Action.Parent.HasValue)
                {
                    var (action, actionType) = ctx.Control.FindActionAndType(Action.Parent.Value);
                    actionType.BackStop(ctx);
                }
            }
            else
            {
                Action.Status = ActionStatus.Stopped;
                ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateToStop, Action));

                if (Action.Parent.HasValue)
                {
                    var (action, actionType) = ctx.Control.FindActionAndType(Action.Parent.Value);
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
            Assert.Enum.In(Action.Status, new[] {
                ActionStatus.ToStop
            }, $"ActionScript in wrong status to Completed! {Action.ActionId}-{Action.Label}");

            Action.Status = ActionStatus.Stopped;
            ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, Action));

            if (Action.Parent.HasValue)
            {
                var (action, actionType) = ctx.Control.FindActionAndType(Action.Parent.Value);
                actionType.BackSetStopped(ctx);
            }
        }

        public override void BackSetStopped(CommandContext ctx)
        {
            throw new RunnerException("ActionScript shound't never stopped from back!");
        }

        public override void SetBreakPoint(CommandContext ctx)
        {
            Assert.Enum.In(Action.Status, new[] {
                ActionStatus.Waiting,
                ActionStatus.Stopped,
                ActionStatus.Error
            }, $"ActionScript in wrong status to set breakpoint! {Action.ActionId}-{Action.Label}");

            Action.BreakPoint = true;
            ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateBreakPoint, Action));
        }

        public override void CleanBreakPoint(CommandContext ctx)
        {
            Action.BreakPoint = false;
            ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateBreakPoint, Action));
        }

        public override void BackBreakPoint(CommandContext ctx)
        {
            throw new RunnerException("ActionScript shound't never breakpoint from back!");
        }

        public override void BuildData(DataContext ctx)
        {
            ctx.Parents.Add(this);
            if (Action.Parent.HasValue)
            {
                var (_, actionType) = ctx.Control.FindActionAndType(Action.Parent.Value);
                actionType.BackBuildData(ctx, Action.ActionId);
            }
        }

        public override void BackBuildData(DataContext ctx, int actionChildId)
        {
            throw new RunnerException("ActionScript shound't never call build data from back!");
        }
    }
}
