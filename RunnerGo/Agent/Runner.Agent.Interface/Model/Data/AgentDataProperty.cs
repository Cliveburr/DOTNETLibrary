
namespace Runner.Agent.Interface.Model.Data
{
    public class AgentDataProperty
    {
        public required string Name { get; set; }
        public required AgentDataTypeEnum Type { get; set; }
        public AgentDataValue? Value { get; set; }
    }
}
