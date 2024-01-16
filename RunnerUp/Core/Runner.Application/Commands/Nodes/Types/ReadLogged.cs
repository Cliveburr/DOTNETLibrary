using Runner.Application.Commands.Nodes.Types.DTO;
using Runner.Kernel.Events.Command;

namespace Runner.Application.Commands.Nodes.Types
{
    public record ReadLogged : ICommandResult<List<AppDTO>>;
}
