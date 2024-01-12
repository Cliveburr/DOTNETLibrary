using Runner.Domain.Entities.Identity;
using Runner.Kernel.Events.Write;

namespace Runner.Domain.Write.Identity
{
    public record class UserInsert(User User) : IWrite;
}
