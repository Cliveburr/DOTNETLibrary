
namespace Runner.Agent.Interface.Model
{
    public class GetScriptRequest
    {
        public required string ScriptId { get; set; }
        public required int Version { get; set; }
    }
}
