using Runner.Domain.Entities.Authentication;
using Runner.Kernel.Events.Write;

namespace Runner.Domain.Write.Authentication
{
    public record AccessTokenInsert(AccessToken accessToken) : IWrite;
}
