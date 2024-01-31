
namespace Runner.Business.Actions
{
    public class Action
    {
        public int ActionId { get; set; }
        public required string Label { get; set; }
        public string? AgentPool { get; set; }
        public List<string>? Tags { get; set; }
        public ActionStatus Status { get; set; }
        public ActionType Type { get; set; }
        public bool WithCursor { get; set; }
        public bool BreakPoint { get; set; }
        public List<int>? Childs { get; set; }
        public int? Parent { get; set; }
    }
}
