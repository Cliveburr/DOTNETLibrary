using Runner.Script.Interface.Data;

namespace Runner.Agent.Interface.Model
{
    public class RunScriptResponse
    {
        public bool IsSuccess { get; set; }
        public bool ContinueOnError { get; set; }
        public string? ErrorMessage { get; set; }
        public List<DataWriterChanges>? Output { get; set; }
    }
}
