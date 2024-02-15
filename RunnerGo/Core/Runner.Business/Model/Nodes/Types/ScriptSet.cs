using Runner.Business.Datas2.Model;

namespace Runner.Business.Model.Nodes.Types
{
    public class ScriptSet
    {
        public required string Name { get; set; }
        public int Version { get; set; }
        public required string Assembly { get; set; }
        public required string FullTypeName { get; set; }
        public required List<DataProperty> Input { get; set; }
    }
}
