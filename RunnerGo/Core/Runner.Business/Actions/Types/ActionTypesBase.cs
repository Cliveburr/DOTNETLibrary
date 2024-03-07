
namespace Runner.Business.Actions.Types
{
    public abstract class ActionTypesBase
    {
        public Action Action { get; init; }

        public ActionTypesBase(Action action)
        {
            Action = action;
        }

        public abstract void Run(CommandContext ctx);
        public abstract FowardRunResult FowardRun(CommandContext ctx);
        public abstract void BackRun(CommandContext ctx);
        public abstract void SetRunning(CommandContext ctx);
        public abstract void BackSetRunning(CommandContext ctx);
        public abstract void SetCompleted(CommandContext ctx);
        public abstract void BackSetCompleted(CommandContext ctx, int actionChildId);
        public abstract void SetError(CommandContext ctx, string error);
        public abstract void BackSetError(CommandContext ctx, string error);
        public abstract void Stop(CommandContext ctx);
        public abstract void BackStop(CommandContext ctx);
        public abstract void SetStopped(CommandContext ctx);
        public abstract void BackSetStopped(CommandContext ctx);
        public abstract void SetBreakPoint(CommandContext ctx);
        public abstract void CleanBreakPoint(CommandContext ctx);
        public abstract void BackBreakPoint(CommandContext ctx);
        public abstract void BuildData(DataContext ctx);
        public abstract void BackBuildData(DataContext ctx, int actionChildId);

        public enum FowardRunResult
        {
            Running = 0,
            WasBreakPoint = 1,
            WasCompleted = 2
        }

        //protected IEnumerable<CommandEffect> PropagateBackRun()
        //{
        //    if (_action.Parent.HasValue)
        //    {
        //        var (action, actionType) = _control.FindActionAndType(_action.Parent.Value);

        //        foreach (var command in actionType.BackRun())
        //        {
        //            yield return command;
        //        };
        //    }
        //}

        //protected IEnumerable<CommandEffect> PropagateBackSetRunning()
        //{
        //    if (_action.Parent.HasValue)
        //    {
        //        var (action, actionType) = _control.FindActionAndType(_action.Parent.Value);

        //        foreach (var command in actionType.BackSetRunning())
        //        {
        //            yield return command;
        //        };
        //    }
        //}

        //protected IEnumerable<CommandEffect> PropagateBackSetCompleted()
        //{
        //    if (_action.Parent.HasValue)
        //    {
        //        var (action, actionType) = _control.FindActionAndType(_action.Parent.Value);

        //        foreach (var command in actionType.BackSetCompleted(_action.ActionId))
        //        {
        //            yield return command;
        //        };
        //    }
        //}


        //protected IEnumerable<CommandEffect> PropagateBackSetError()
        //{
        //    if (_action.Parent.HasValue)
        //    {
        //        var (action, actionType) = _control.FindActionAndType(_action.Parent.Value);

        //        foreach (var command in actionType.BackSetError())
        //        {
        //            yield return command;
        //        };
        //    }
        //}

        //protected IEnumerable<CommandEffect> PropagateBackStop()
        //{
        //    if (_action.Parent.HasValue)
        //    {
        //        var (action, actionType) = _control.FindActionAndType(_action.Parent.Value);

        //        foreach (var command in actionType.BackStop())
        //        {
        //            yield return command;
        //        };
        //    }
        //}

        //protected IEnumerable<CommandEffect> PropagateBackSetStopped()
        //{
        //    if (_action.Parent.HasValue)
        //    {
        //        var (action, actionType) = _control.FindActionAndType(_action.Parent.Value);

        //        foreach (var command in actionType.BackSetStopped())
        //        {
        //            yield return command;
        //        };
        //    }
        //}

        //protected IEnumerable<CommandEffect> PropagateBackBreakPoint()
        //{
        //    if (_action.Parent.HasValue)
        //    {
        //        var (action, actionType) = _control.FindActionAndType(_action.Parent.Value);

        //        foreach (var command in actionType.BackBreakPoint())
        //        {
        //            yield return command;
        //        };
        //    }
        //}
    }
}
