using Runner.Domain.Entities;
using Runner.Domain.Entities.Authentication;
using Runner.Kernel.Events.Read;

namespace Runner.Domain.Read.Authentication
{
    public record ReadByUserId(EntityId UserId, AccessTokenType Type) : IRead<AccessToken?>;
}
