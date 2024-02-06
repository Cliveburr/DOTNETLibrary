using Runner.Business.Datas.Model;

namespace Runner.Business.Model.Nodes.Types
{
    public class ScriptSet
    {
        public required string Name { get; set; }
        public int Version { get; set; }
        public required string Assembly { get; set; }
        public required string FullTypeName { get; set; }
        public required List<DataTypeProperty> InputTypes { get; set; }
        public required List<DataTypeProperty> OutputTypes { get; set; }
    }
}
