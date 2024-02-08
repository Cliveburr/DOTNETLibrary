using Runner.Script.Interface.Model.Data;

namespace Runner.Script.Interface.Scripts
{
    public class ScriptRunContext
    {
        public required ScriptData Data { get; set; }
        public required Func<string, Task> Log { get; set; }
        public CancellationToken CancellationToken { get; set; }
    }
}
