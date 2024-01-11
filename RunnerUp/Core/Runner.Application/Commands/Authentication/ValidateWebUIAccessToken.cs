using Runner.Kernel.Events.Command;

namespace Runner.Application.Commands.Authentication
{
    public record ValidateWebUIAccessToken(string Token) : ICommandResult<bool>;
}
