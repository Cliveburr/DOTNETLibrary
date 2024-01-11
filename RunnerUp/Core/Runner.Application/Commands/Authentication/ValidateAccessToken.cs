using Runner.Domain.Entities.Authentication;
using Runner.Kernel.Events.Command;

namespace Runner.Application.Commands.Authentication
{
    internal record ValidateAccessToken(string Token, AccessTokenType Type) : ICommandResult<bool>;
}
