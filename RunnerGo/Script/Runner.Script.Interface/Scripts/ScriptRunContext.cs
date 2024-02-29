using Runner.Script.Interface.Model.Data;

namespace Runner.Script.Interface.Scripts
{
    public class ScriptRunContext
    {
        public required ScriptData Data { get; init; }
        public required Func<string, Task> Log { get; init; }
        public CancellationToken CancellationToken { get; init; }
        public required IWorkspace Workspace { get; init; }
    }
}
