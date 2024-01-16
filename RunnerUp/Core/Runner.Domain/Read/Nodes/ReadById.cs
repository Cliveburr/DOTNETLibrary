using Runner.Domain.Entities;
using Runner.Domain.Entities.Nodes;
using Runner.Kernel.Events.Read;

namespace Runner.Domain.Read.Nodes
{
    public record ReadByListId(List<EntityId> NodeIds) : IRead<List<Node>>;
}
