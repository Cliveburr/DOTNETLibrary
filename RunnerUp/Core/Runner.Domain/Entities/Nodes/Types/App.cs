using Runner.Domain.Entities;

namespace Runner.Business.Entities.Nodes.Types
{
    public class App
    {
        public EntityId AppId { get; set; }
        public EntityId NodeId { get; set; }
        public required EntityId OwnerId { get; set; }
    }
}
