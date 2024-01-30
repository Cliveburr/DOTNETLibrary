
namespace Runner.Script.Interface.Model.Data
{
    public class DataTypeProperty
    {
        public required string Name { get; set; }
        public required DataTypeEnum Type { get; set; }
        public object? Default { get; set; }
        public bool IsRequired { get; set; }
    }
}
