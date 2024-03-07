
namespace Runner.Business.Actions
{
    public partial class ActionControl
    {
        public List<CommandEffect> Run()
        {
            var rootAction = FindAction(EntityRun.RootActionId);
            var ctx = new CommandContext(this, new List<CommandEffect>());
            foreach (var action in FindActionsAbleToRun(rootAction))
            {
                var actionType = FindActionType(action);
                actionType.Run(ctx);
            }
            return ctx.Effects;
        }

        public List<CommandEffect> Run(int actionId)
        {
            var action = FindAction(actionId);
            var actionType = FindActionType(action);

            var ctx = new CommandContext(this, new List<CommandEffect>());
            actionType.Run(ctx);
            return ctx.Effects;
        }

        public List<CommandEffect> SetRunning(int actionId)
        {
            var action = FindAction(actionId);
            var actionType = FindActionType(action);

            var ctx = new CommandContext(this, new List<CommandEffect>());
            actionType.SetRunning(ctx);
            return ctx.Effects;
        }

        public List<CommandEffect> SetCompleted(int actionId)
        {
            var action = FindAction(actionId);
            var actionType = FindActionType(action);

            var ctx = new CommandContext(this, new List<CommandEffect>());
            actionType.SetCompleted(ctx);
            return ctx.Effects;
        }

        public List<CommandEffect> SetError(int actionId, string error)
        {
            var action = FindAction(actionId);
            var actionType = FindActionType(action);

            var ctx = new CommandContext(this, new List<CommandEffect>());
            actionType.SetError(ctx, error);
            return ctx.Effects;
        }

        public List<CommandEffect> Stop(int actionId)
        {
            var action = FindAction(actionId);
            var actionType = FindActionType(action);

            var ctx = new CommandContext(this, new List<CommandEffect>());
            actionType.Stop(ctx);
            return ctx.Effects;
        }

        public List<CommandEffect> SetStopped(int actionId)
        {
            var action = FindAction(actionId);
            var actionType = FindActionType(action);

            var ctx = new CommandContext(this, new List<CommandEffect>());
            actionType.SetStopped(ctx);
            return ctx.Effects;
        }

        public List<CommandEffect> SetBreakPoint(int actionId)
        {
            var action = FindAction(actionId);
            var actionType = FindActionType(action);

            var ctx = new CommandContext(this, new List<CommandEffect>());
            actionType.SetBreakPoint(ctx);
            return ctx.Effects;
        }

        public List<CommandEffect> CleanBreakPoint(int actionId)
        {
            var action = FindAction(actionId);
            var actionType = FindActionType(action);

            var ctx = new CommandContext(this, new List<CommandEffect>());
            actionType.CleanBreakPoint(ctx);
            return ctx.Effects;
        }
    }
}
