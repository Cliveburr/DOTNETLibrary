
namespace Runner.Business.Actions
{
    public record CommandContext(ActionControl Control, List<CommandEffect> Effects);
}
