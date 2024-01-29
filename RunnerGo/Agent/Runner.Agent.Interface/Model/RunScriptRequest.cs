
namespace Runner.Agent.Interface.Model
{
    public class RunScriptRequest
    {
        public required string ScriptId { get; set; }
        public required int Version { get; set; }
        public required string Assembly { get; set; }
        public required string Type { get; set; }
        public required Dictionary<string, object?> Input { get; set; }
    }
}
