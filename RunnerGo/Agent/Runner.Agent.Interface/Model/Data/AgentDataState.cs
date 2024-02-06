
namespace Runner.Agent.Interface.Model.Data
{
    public class AgentDataState
    {
        public required AgentDataProperty Property { get; set; }
        public AgentDataStateType State { get; set; }
    }
}
