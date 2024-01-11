using Runner.Domain.Entities.Authentication;
using Runner.Kernel.Events.Read;

namespace Runner.Domain.Read.Authentication
{
    public record ReadByToken(string Token, AccessTokenType Type) : IRead<AccessToken?>;
}
