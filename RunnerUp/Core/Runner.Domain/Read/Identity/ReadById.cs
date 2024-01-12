using Runner.Domain.Entities;
using Runner.Domain.Entities.Identity;
using Runner.Kernel.Events.Read;

namespace Runner.Domain.Read.Identity
{
    public record ReadById(EntityId UserId) : IRead<User?>;
}
