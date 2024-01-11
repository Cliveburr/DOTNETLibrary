using Runner.Domain.Entities.Authentication;
using Runner.Kernel.Events.Read;

namespace Runner.Domain.Read.Authentication
{
    public record ReadByUserId(Guid UserId, AccessTokenType Type) : IRead<AccessToken?>;
}
