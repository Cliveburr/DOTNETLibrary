
namespace Runner.Agent.Interface.Model.Data
{
    public class DataProperty
    {
        public required string Name { get; set; }
        public required DataTypeEnum Type { get; set; }
        public object? Value { get; set; }
    }
}
