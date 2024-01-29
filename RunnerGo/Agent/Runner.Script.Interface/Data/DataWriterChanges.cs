
namespace Runner.Script.Interface.Data
{
    public class DataWriterChanges
    {
        public required string Name { get; set; }
        public DataWriterCommand Command { get; set; }
        public DataTypeEnum Type { get; set; }
        public object? Value { get; set; }
    }
}
