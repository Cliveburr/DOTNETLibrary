
namespace Runner.Script.Interface.Scripts
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ScriptAttribute : Attribute
    {
        public int Version { get; init; }
        public string Name { get; init; }
        public Type? Input { get; set; }

        public ScriptAttribute(int version, string name)
        {
            Version = version;
            Name = name;
        }

        public ScriptAttribute(int version, string name, Type input)
        {
            Version = version;
            Name = name;
            Input = input;
        }
    }
}
