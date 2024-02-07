
namespace Runner.Agent.Interface.Model.Data
{
    public class AgentDataTransfer
    {
        public AgentDataPropertyString[]? Strings { get; set; }
        public AgentDataPropertyStringList[]? StringLists { get; set; }
        public AgentDataPropertyNodePath[]? NodePaths { get; set; }
    }
}
