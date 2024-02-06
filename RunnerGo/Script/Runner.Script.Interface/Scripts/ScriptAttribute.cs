using Runner.Script.Interface.Model.Data;

namespace Runner.Script.Interface.Scripts
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ScriptAttribute : Attribute
    {
        public int Version { get; init; }
        public string Name { get; init; }
        public List<ScriptDataTypeProperty>? InputTypes { get; set; }
        public List<ScriptDataTypeProperty>? OutputTypes { get; set; }

        public ScriptAttribute(int version, string name)
        {
            Version = version;
            Name = name;
        }

        public ScriptAttribute(int version, string name, List<ScriptDataTypeProperty>? input = null, List<ScriptDataTypeProperty>? output = null)
        {
            Version = version;
            Name = name;
        }
    }
}
