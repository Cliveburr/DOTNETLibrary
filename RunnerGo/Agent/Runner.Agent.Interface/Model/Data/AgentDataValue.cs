
namespace Runner.Agent.Interface.Model.Data
{
    public class AgentDataValue
    {
        public string? StringValue { get; set; }
        public int? IntValue { get; set; }
        public List<string>? StringListValue { get; set; }

        public string? NodePath { get; set; }
        public List<AgentDataProperty>? DataExpand { get; set; }
    }
}
