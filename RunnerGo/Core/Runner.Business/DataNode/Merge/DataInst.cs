using Runner.Business.Entities.Nodes.Types;

namespace Runner.Business.DataNode.Merge
{
    public class DataInst
    {
        public required string Name { get; set; }
        public required DataTypeEnum Type { get; set; }
        public object? Default { get; set; }
        public bool IsRequired { get; set; }
        public object? Value { get; set; }
    }
}
