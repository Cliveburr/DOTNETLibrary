
namespace Runner.Business.Actions.Types
{
    public class ActionContainer : ActionTypesBase
    {
        public ActionContainer(Action action)
            : base(action)
        {
        }

        public override FowardRunResult FowardRun(CommandContext ctx)
        {
            Assert.Enum.In(Action.Status, new[] {
                ActionStatus.Waiting
            }, $"ActionContainer in wrong status to Cursor! {Action.ActionId}-{Action.Label}");

            Assert.MustFalse(Action.WithCursor, $"ActionContainer already with Cursor! {Action.ActionId}-{Action.Label}");

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
                Assert.MustNotNull(Action.Childs, $"ActionContainer with invalid Childs! {Action.ActionId}-{Action.Label}");

                if (Action.Childs.Any())
                {
                    var firstActionIdChild = Action.Childs.First();
                    var (nextAction, nextActionType) = ctx.Control.FindActionAndType(firstActionIdChild);

                    var childFowardRunResult = nextActionType.FowardRun(ctx);
                    switch (childFowardRunResult)
                    {
                        case FowardRunResult.WasBreakPoint:
                            {
                                Action.Status = ActionStatus.Stopped;
                                ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, Action));
                                return FowardRunResult.WasBreakPoint;
                            }
                        case FowardRunResult.WasCompleted:
                            {
                                Action.Status = ActionStatus.Completed;
                                ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, Action));
                                return FowardRunResult.WasCompleted;
                            }
                        case FowardRunResult.Running:
                            {
                                Action.Status = ActionStatus.Running;
                                ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, Action));
                                return FowardRunResult.Running;
                            }
                        default: throw new RunnerException($"ActionContainer receive invalid foward run result! {Action.ActionId}-{Action.Label}");
                    }
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
            }, $"ActionContainer in wrong status to Run! {Action.ActionId}-{Action.Label}");

            Assert.MustTrue(Action.WithCursor, $"ActionContainer in without cursor to Run! {Action.ActionId}-{Action.Label}");

            Action.WithCursor = false;
            ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateWithCursor, Action));

            Assert.MustNotNull(Action.Childs, $"ActionParallel with invalid Childs! {Action.ActionId}-{Action.Label}");

            if (Action.Childs.Any())
            {
                var firstActionIdChild = Action.Childs.First();
                var (nextAction, nextActionType) = ctx.Control.FindActionAndType(firstActionIdChild);

                ExecuteFowardRun(ctx, nextActionType);
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

        private void ExecuteFowardRun(CommandContext ctx, ActionTypesBase nextActionType)
        {
            var childFowardRunResult = nextActionType.FowardRun(ctx);
            switch (childFowardRunResult)
            {
                case FowardRunResult.WasBreakPoint:
                    {
                        if (Action.Status != ActionStatus.Stopped)
                        {
                            Action.Status = ActionStatus.Stopped;
                            ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, Action));

                            if (Action.Parent.HasValue)
                            {
                                var (action, actionType) = ctx.Control.FindActionAndType(Action.Parent.Value);
                                actionType.BackBreakPoint(ctx);
                            }
                        }
                        break;
                    }
                case FowardRunResult.WasCompleted:
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
                        break;
                    }
                case FowardRunResult.Running:
                    {
                        if (Action.Status != ActionStatus.Running)
                        {
                            Action.Status = ActionStatus.Running;
                            ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, Action));

                            if (Action.Parent.HasValue)
                            {
                                var (action, actionType) = ctx.Control.FindActionAndType(Action.Parent.Value);
                                actionType.BackRun(ctx);
                            }
                        }
                        break;
                    }
                default: throw new RunnerException($"ActionContainer receive invalid foward run result! {Action.ActionId}-{Action.Label}");
            }
        }

        public override void BackRun(CommandContext ctx)
        {
            if (Action.Status != ActionStatus.Running)
            {
                Assert.Enum.In(Action.Status, new[] {
                    ActionStatus.Stopped,
                    ActionStatus.Error
                }, $"ActionContainer container in wrong status to BackRun! {Action.ActionId}-{Action.Label}");

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
            throw new RunnerException("ActionContainer shound't never call running!");
        }

        public override void BackSetRunning(CommandContext ctx)
        {
            Assert.Enum.In(Action.Status, new[] {
                ActionStatus.Running
            }, $"ActionContainer in wrong status to BackSetRunning! {Action.ActionId}-{Action.Label}");

            if (Action.Parent.HasValue)
            {
                var (action, actionType) = ctx.Control.FindActionAndType(Action.Parent.Value);
                actionType.BackSetRunning(ctx);
            }
        }

        public override void SetCompleted(CommandContext ctx)
        {
            throw new RunnerException("ActionContainer shound't never call completed!");
        }

        public override void BackSetCompleted(CommandContext ctx, int actionChildId)
        {
            Assert.Enum.In(Action.Status, new[] {
                ActionStatus.Running,
                ActionStatus.ToStop
            }, $"ActionContainer in wrong status to BackSetCompleted! {Action.ActionId}-{Action.Label}");

            Assert.MustNotNull(Action.Childs, $"Action container with invalid Childs! {Action.ActionId}-{Action.Label}");

            var currentChildIndex = Action.Childs.IndexOf(actionChildId);
            Assert.Number.InRange(currentChildIndex, 0, Action.Childs.Count - 1, $"Invalid child index in parent! {Action.ActionId}-{Action.Label}, ActionChildId: {actionChildId}");

            var nextChildIndex = currentChildIndex + 1;
            if (nextChildIndex == Action.Childs.Count)
            {
                Action.Status = ActionStatus.Completed;
                ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, Action));

                if (Action.Parent.HasValue)
                {
                    var (action, actionType) = ctx.Control.FindActionAndType(Action.Parent.Value);
                    actionType.BackSetCompleted(ctx, Action.ActionId);
                }
            }
            else
            {
                var nextActionId = Action.Childs[nextChildIndex];
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
            Assert.Enum.In(Action.Status, new[] {
                ActionStatus.Running,
                ActionStatus.ToStop
            }, $"Action container in wrong status to BackSetError! {Action.ActionId}-{Action.Label}");

            Action.Status = ActionStatus.Error;
            ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, Action));

            if (Action.Parent.HasValue)
            {
                var (action, actionType) = ctx.Control.FindActionAndType(Action.Parent.Value);
                actionType.BackSetError(ctx);
            }
        }

        public override void Stop(CommandContext ctx)
        {
            throw new RunnerException("ActionContainer shound't never call stop!");
        }

        public override void BackStop(CommandContext ctx)
        {
            Assert.Enum.In(Action.Status, new[] {
                ActionStatus.Running
            }, $"ActionContainer in wrong status to Stop! {Action.ActionId}-{Action.Label}");

            if (Action.Parent.HasValue)
            {
                var (action, actionType) = ctx.Control.FindActionAndType(Action.Parent.Value);
                actionType.BackStop(ctx);
            }
        }

        public override void SetStopped(CommandContext ctx)
        {
            throw new RunnerException("ActionContainer shound't never call stopped!");
        }

        public override void BackSetStopped(CommandContext ctx)
        {
            Assert.Enum.In(Action.Status, new[] {
                ActionStatus.Running
            }, $"ActionContainer in wrong status to Stopped! {Action.ActionId}-{Action.Label}");

            Action.Status = ActionStatus.Stopped;
            ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, Action));

            if (Action.Parent.HasValue)
            {
                var (action, actionType) = ctx.Control.FindActionAndType(Action.Parent.Value);
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
            if (Action.Status != ActionStatus.Stopped)
            {
                Action.Status = ActionStatus.Stopped;
                ctx.Effects.Add(new CommandEffect(ComandEffectType.ActionUpdateStatus, Action));
            }

            if (Action.Parent.HasValue)
            {
                var (action, actionType) = ctx.Control.FindActionAndType(Action.Parent.Value);
                actionType.BackBreakPoint(ctx);
            }
        }

        public override void BuildData(DataContext ctx)
        {
            Assert.Enum.In(Action.Status, new[] {
                ActionStatus.Completed
            }, $"ActionContainer in wrong status to BuildData! {Action.ActionId}-{Action.Label}");

            Assert.MustNotNull(Action.Childs, $"Action container with invalid Childs! {Action.ActionId}-{Action.Label}");
            var lastChildIndex = Action.Childs.Count - 1;

            var lastActionId = Action.Childs[lastChildIndex];
            var (_, lastActionType) = ctx.Control.FindActionAndType(lastActionId);

            lastActionType.BuildData(ctx);
        }

        public override void BackBuildData(DataContext ctx, int actionChildId)
        {
            Assert.MustNotNull(Action.Childs, $"Action container with invalid Childs! {Action.ActionId}-{Action.Label}");

            var currentChildIndex = Action.Childs.IndexOf(actionChildId);
            Assert.Number.InRange(currentChildIndex, 0, Action.Childs.Count - 1, $"Invalid child index in parent! {Action.ActionId}-{Action.Label}, ActionChildId: {actionChildId}");

            var previousChildIndex = currentChildIndex - 1;
            if (previousChildIndex > -1)
            {
                var previousActionId = Action.Childs[previousChildIndex];
                var (_, previousActionType) = ctx.Control.FindActionAndType(previousActionId);

                previousActionType.BuildData(ctx);
            }
            else
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
}
