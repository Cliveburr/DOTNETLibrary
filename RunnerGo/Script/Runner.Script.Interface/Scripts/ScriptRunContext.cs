using Runner.Script.Interface.Model.Data;
using Runner.Script.Interface.Workspaces;

namespace Runner.Script.Interface.Scripts
{
    public class ScriptRunContext
    {
        public required string SiteUrl { get; set; }
        public required ScriptData Data { get; init; }
        public required Func<string, Task> Log { get; init; }
        public CancellationToken CancellationToken { get; init; }
        public required Workspace Workspace { get; init; }
    }
}
