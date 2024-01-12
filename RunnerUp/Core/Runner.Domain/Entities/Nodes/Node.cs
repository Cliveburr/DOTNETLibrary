
namespace Runner.Domain.Entities.Nodes
{
    public class Node
    {
        public EntityId NodeId { get; set; }
        public required string Name { get; set; }
        public NodeType Type { get; set; }
        public EntityId? ParentId { get; set; }
    }
}
