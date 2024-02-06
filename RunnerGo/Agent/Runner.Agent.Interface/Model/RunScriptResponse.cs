using Runner.Agent.Interface.Model.Data;

namespace Runner.Agent.Interface.Model
{
    public class RunScriptResponse
    {
        public bool IsSuccess { get; set; }
        public bool ContinueOnError { get; set; }
        public string? ErrorMessage { get; set; }
        public List<AgentDataState>? Data { get; set; }
    }
}
