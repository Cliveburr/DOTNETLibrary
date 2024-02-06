namespace Runner.Script.Interface.Model.Data
{
    public class ScriptDataProperty
    {
        public required string Name { get; set; }
        public required ScriptDataTypeEnum Type { get; set; }
        public object? Value { get; set; }
    }
}
