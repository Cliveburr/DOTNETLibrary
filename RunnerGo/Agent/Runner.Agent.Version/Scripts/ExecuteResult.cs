using Runner.Agent.Interface.Model.Data;

namespace Runner.Agent.Version.Scripts
{
    public record ExecuteResult
    {
        public bool IsSuccess { get; set; }
        public bool ContinueOnError { get; set; }
        public string? ErrorMessage { get; set; }
        public List<AgentDataState>? Data { get; set; }
    }
}
