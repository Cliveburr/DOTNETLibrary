
namespace Runner.Script.Interface.Model.Data
{
    public class ScriptDataTypeProperty
    {
        public required string Name { get; set; }
        public required ScriptDataTypeEnum Type { get; set; }
        public object? Default { get; set; }
        public bool IsRequired { get; set; }
    }
}
