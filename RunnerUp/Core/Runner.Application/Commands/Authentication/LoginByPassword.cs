using Runner.Kernel.Events.Command;

namespace Runner.Application.Commands.Authentication
{
    public record LoginByPassword(string Name, string Password) : ICommandResult<string>;
}
