using Runner.Agent.Interface.Model.Data;

namespace Runner.Agent.Interface.Model
{
    public class RunScriptRequest
    {
        public required string ScriptContentId { get; set; }
        public required string Assembly { get; set; }
        public required string FullTypeName { get; set; }
        public List<AgentDataProperty>? Data { get; set; }
    }
}
