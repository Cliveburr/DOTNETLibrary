
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
            Assert.Enum.In(Action.Status, new[] {
                ActionStatus.Waiting
            }, $"ActionParallel in wrong status to Cursor! {Action.ActionId}-{Action.Label}");

            Assert.MustFalse(Action.WithCursor, $"ActionParallel already with Cursor! {Action.ActionId}-{Action.Label}");

            if (Action.BreakPoint)
            {
                Action.WithCursor = true;
                ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateWithCursor, Action));

                Action.Status = ActionStatus.Stopped;
                ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, Action));

                return FowardRunResult.WasBreakPoint;
            }
            else
            {
                if (Action.Childs is not null && Action.Childs.Any())
                {
                    var fowardRunResults = new List<FowardRunResult>();
                    foreach (var childAction in Action.Childs)
                    {
                        var (nextAction, nextActionType) = ctx.Control.FindActionAndType(childAction);
                        fowardRunResults.Add(nextActionType.FowardRun(ctx));
                    }

                    var isSomeoneRunning = fowardRunResults
                        .Any(cs => cs == FowardRunResult.Running);
                    if (isSomeoneRunning)
                    {
                        Action.Status = ActionStatus.Running;
                        ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, Action));
                        return FowardRunResult.Running;
                    }

                    var isSomeoneBreakPoint = fowardRunResults
                        .Any(cs => cs == FowardRunResult.WasBreakPoint);
                    if (isSomeoneBreakPoint)
                    {
                        Action.Status = ActionStatus.Stopped;
                        ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, Action));
                        return FowardRunResult.WasBreakPoint;
                    }

                    return FowardRunResult.WasCompleted;
                }
                else
                {
                    Action.Status = ActionStatus.Completed;
                    ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, Action));

                    return FowardRunResult.WasCompleted;
                }
            }
        }

        public override void Run(CommandContext ctx)
        {
            Assert.Enum.In(Action.Status, new[] {
                ActionStatus.Waiting,
                ActionStatus.Stopped,
                ActionStatus.Error
            }, $"ActionParallel in wrong status to Run! {Action.ActionId}-{Action.Label}");

            Assert.MustTrue(Action.WithCursor, $"ActionParallel in without cursor to Run! {Action.ActionId}-{Action.Label}");

            Action.WithCursor = false;
            ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateWithCursor, Action));

            if (Action.Childs is not null && Action.Childs.Any())
            {
                foreach (var childAction in Action.Childs)
                {
                    var (nextAction, nextActionType) = ctx.Control.FindActionAndType(childAction);
                    _ = nextActionType.FowardRun(ctx);
                }

                CheckStatus(ctx);
            }
            else
            {
                Action.Status = ActionStatus.Completed;
                ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, Action));

                if (Action.Parent.HasValue)
                {
                    var (action, actionType) = ctx.Control.FindActionAndType(Action.Parent.Value);
                    actionType.BackSetCompleted(ctx, Action.ActionId);
                }
            }
        }

        public override void BackRun(CommandContext ctx)
        {
            if (Action.Status != ActionStatus.Running)
            {
                Action.Status = ActionStatus.Running;
                ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, Action));
            }

            if (Action.Parent.HasValue)
            {
                var (action, actionType) = ctx.Control.FindActionAndType(Action.Parent.Value);
                actionType.BackRun(ctx);
            }
        }

        public override void SetRunning(CommandContext ctx)
        {
            throw new RunnerException("ActionParallel shound't never call running!");
        }

        public override void BackSetRunning(CommandContext ctx)
        {
            Assert.Enum.In(Action.Status, new[] {
                ActionStatus.Running
            }, $"ActionParallel in wrong status to BackSetRunning! {Action.ActionId}-{Action.Label}");

            CheckStatus(ctx);
        }

        public override void SetCompleted(CommandContext ctx)
        {
            throw new RunnerException("ActionParallel shound't never call completed!");
        }

        public override void BackSetCompleted(CommandContext ctx, int actionChildId)
        {
            Assert.Enum.In(Action.Status, new[] {
                ActionStatus.Running,
                ActionStatus.ToStop
            }, $"ActionParallel in wrong status to BackSetCompleted! {Action.ActionId}-{Action.Label}");

            CheckStatus(ctx);
        }

        public override void SetError(CommandContext ctx)
        {
            throw new RunnerException("ActionParallel shound't never call error!");
        }

        public override void BackSetError(CommandContext ctx)
        {
            Assert.Enum.In(Action.Status, new[] {
                ActionStatus.Running,
                ActionStatus.ToStop
            }, $"Action container in wrong status to BackSetError! {Action.ActionId}-{Action.Label}");

            CheckStatus(ctx);
        }


        public override void Stop(CommandContext ctx)
        {
            throw new RunnerException("ActionParallel shound't never call stop!");
        }

        public override void BackStop(CommandContext ctx)
        {
            Assert.Enum.In(Action.Status, new[] {
                ActionStatus.Running
            }, $"ActionParallel in wrong status to Stop! {Action.ActionId}-{Action.Label}");

            if (Action.Parent.HasValue)
            {
                var (action, actionType) = ctx.Control.FindActionAndType(Action.Parent.Value);
                actionType.BackStop(ctx);
            }
        }

        public override void SetStopped(CommandContext ctx)
        {
            throw new RunnerException("ActionParallel shound't never call stopped!");
        }

        public override void BackSetStopped(CommandContext ctx)
        {
            Assert.Enum.In(Action.Status, new[] {
                ActionStatus.Running
            }, $"ActionParallel in wrong status to Stopped! {Action.ActionId}-{Action.Label}");

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

            if (Action.Parent.HasValue)
            {
                var (action, actionType) = ctx.Control.FindActionAndType(Action.Parent.Value);
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

            Assert.MustNotNull(Action.Childs, $"ActionParallel with invalid Childs! {Action.ActionId}-{Action.Label}");

            var childsStatus = Action.Childs
                .Select(i => ctx.Control.FindAction(i).Status)
                .ToList();

            var isSomeoneRunning = childsStatus
                .Any(cs => cs == ActionStatus.Running ||
                    cs == ActionStatus.ToStop ||
                    cs == ActionStatus.ToRun);
            if (isSomeoneRunning)
            {
                if (Action.Status != ActionStatus.Running)
                {
                    Action.Status = ActionStatus.Running;
                    ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, Action));

                    if (Action.Parent.HasValue)
                    {
                        var (action, actionType) = ctx.Control.FindActionAndType(Action.Parent.Value);
                        actionType.BackSetRunning(ctx);
                    }
                }
                return;
            }

            var isSomeoneError = childsStatus
                .Any(cs => cs == ActionStatus.Error);
            if (isSomeoneError)
            {
                if (Action.Status != ActionStatus.Error)
                {
                    Action.Status = ActionStatus.Error;
                    ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, Action));

                    if (Action.Parent.HasValue)
                    {
                        var (action, actionType) = ctx.Control.FindActionAndType(Action.Parent.Value);
                        actionType.BackSetError(ctx);
                    }
                }
                return;
            }

            var isSomeoneStopped = childsStatus
                .Any(cs => cs == ActionStatus.Stopped);
            if (isSomeoneStopped)
            {
                if (Action.Status != ActionStatus.Stopped)
                {
                    Action.Status = ActionStatus.Stopped;
                    ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, Action));

                    if (Action.Parent.HasValue)
                    {
                        var (action, actionType) = ctx.Control.FindActionAndType(Action.Parent.Value);
                        actionType.BackSetStopped(ctx);
                    }

                }
                return;
            }

            var isAllCompleted = !childsStatus
                .Any(ac => ac != ActionStatus.Completed);
            if (isAllCompleted)
            {
                if (Action.Status != ActionStatus.Completed)
                {
                    Action.Status = ActionStatus.Completed;
                    ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, Action));

                    if (Action.Parent.HasValue)
                    {
                        var (action, actionType) = ctx.Control.FindActionAndType(Action.Parent.Value);
                        actionType.BackSetCompleted(ctx, Action.ActionId);
                    }
                }
                return;
            }
        }

        public override void BuildData(DataContext ctx)
        {
            throw new RunnerException("ActionParallel shound't never call build data!");
        }

        public override void BackBuildData(DataContext ctx, int actionChildId)
        {
            ctx.Parents.Add(this);
            if (Action.Parent.HasValue)
            {
                var (_, actionType) = ctx.Control.FindActionAndType(Action.Parent.Value);
                actionType.BackBuildData(ctx, Action.ActionId);
            }
        }
    }
}
