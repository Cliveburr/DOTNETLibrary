using Runner.Script.Interface.Data;

namespace Runner.Agent.Version.Scripts
{
    public record ExecuteResult
    {
        public bool IsSuccess { get; set; }
        public bool ContinueOnError { get; set; }
        public string? ErrorMessage { get; set; }
        public DataWriter? Output { get; set; }
    }
}
