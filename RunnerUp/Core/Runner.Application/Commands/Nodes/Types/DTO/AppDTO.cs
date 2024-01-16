using Runner.Application.Commands.Nodes.DTO;

namespace Runner.Application.Commands.Nodes.Types.DTO
{
    public class AppDTO
    {
        public required string Name { get; set; }
        public NodeTypeDTO Type { get; set; }
    }
}
