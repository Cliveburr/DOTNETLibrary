using Runner.Script.Interface.Model.Data;

namespace Runner.Script.Interface.Scripts
{
    public class ScriptRunContext
    {
        public bool IsSuccess { get; set; }
        public bool ContinueOnError { get; set; }
        public string? ErrorMessage { get; set; }
        public required Data Data { get; set; }
        public required Func<string, Task> Log { get; set; }
        public CancellationToken CancellationToken { get; set; }
    }
}
