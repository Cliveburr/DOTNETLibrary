
namespace Runner.Agent.Interface.Model
{
    public class ScriptErrorRequest
    {
        public required string Message { get; set; }
        public required string FullError { get; set; }
    }
}
