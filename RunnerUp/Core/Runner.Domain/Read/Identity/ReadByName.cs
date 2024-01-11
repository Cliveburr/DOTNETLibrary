using Runner.Domain.Entities.Identity;
using Runner.Kernel.Events.Read;

namespace Runner.Domain.Read.Identity
{
    public record ReadByName(string Name) : IRead<User?>;
}
