using Runner.Business.Actions.Types;

namespace Runner.Business.Actions
{
    public record CommandContext(ActionControl Control, List<CommandEffect> Effects);

    public record DataContext(ActionControl Control, List<ActionTypesBase> Parents);
}
