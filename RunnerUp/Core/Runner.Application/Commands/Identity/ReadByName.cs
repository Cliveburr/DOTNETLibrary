using Runner.Application.Commands.Identity.DTO;
using Runner.Kernel.Events.Command;

namespace Runner.Application.Commands.Identity
{
    public record ReadByName(string Name) : ICommandResult<UserSafeDTO?>;
}
