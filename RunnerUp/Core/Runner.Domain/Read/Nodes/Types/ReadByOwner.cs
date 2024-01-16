using Runner.Business.Entities.Nodes.Types;
using Runner.Domain.Entities;
using Runner.Kernel.Events.Read;

namespace Runner.Domain.Read.Nodes.Types
{
    public record ReadByOwner(EntityId OwnerId) : IRead<List<App>>;
}
