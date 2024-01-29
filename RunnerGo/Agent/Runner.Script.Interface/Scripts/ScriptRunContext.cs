using Runner.Script.Interface.Data;

namespace Runner.Script.Interface.Scripts
{
    public class ScriptRunContext
    {
        public bool IsSuccess { get; set; }
        public bool ContinueOnError { get; set; }
        public string? ErrorMessage { get; set; }
        public required DataReader Input { get; set; }
        public required DataWriter Output { get; set; }
        public required Func<string, Task> Log { get; set; }
        public CancellationToken CancellationToken { get; set; }
    }
}
