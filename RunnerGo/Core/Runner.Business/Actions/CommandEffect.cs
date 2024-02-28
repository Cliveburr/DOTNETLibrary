
namespace Runner.Business.Actions
{
    public class CommandEffect
    {
        public ComandEffectType Type { get; }
        public Action Action { get; }

        public CommandEffect(ComandEffectType type, Action action)
        {
            Type = type;
            Action = action;
        }

        public override string ToString()
        {
            return $"Command {Type} - Label: {Action.Label}, Status: {Action.Status}, WithCursor: {Action.WithCursor}";
        }
    }

    public enum ComandEffectType
    {
        ActionUpdateStatus = 0,
        ActionUpdateToRun = 1,
        ActionUpdateWithCursor = 2,
        ActionUpdateBreakPoint = 3,
        ActionUpdateToStop = 4,
        ActionUpdateParentRunToRun = 5,
        ActionUpdateParentRunToStop = 6
    }
}
