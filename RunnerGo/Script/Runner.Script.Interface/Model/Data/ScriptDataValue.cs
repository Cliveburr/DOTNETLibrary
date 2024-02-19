
namespace Runner.Script.Interface.Model.Data
{
    public class ScriptDataValue
    {
        public string? StringValue { get; set; }
        public int? IntValue { get; set; }
        public List<string>? StringListValue { get; set; }

        public string? NodePath { get; set; }
        public List<ScriptDataProperty>? DataExpand { get; set; }
    }
}
