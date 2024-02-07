using Runner.Agent.Interface.Model.Data;

namespace Runner.Agent.Interface.Model
{
    public class RunScriptResponse
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public AgentDataTransfer? OutputData { get; set; }
    }
}
