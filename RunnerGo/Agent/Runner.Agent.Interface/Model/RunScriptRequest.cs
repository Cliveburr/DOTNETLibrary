using Runner.Agent.Interface.Model.Data;

namespace Runner.Agent.Interface.Model
{
    public class RunScriptRequest
    {
        public required string ScriptId { get; set; }
        public required int Version { get; set; }
        public required string Assembly { get; set; }
        public required string FullTypeName { get; set; }
        public required List<DataProperty> Data { get; set; }
    }
}
