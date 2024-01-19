
namespace Runner.Agent.Interface.Model
{
    public class RegisterRequest
    {
        public required string MachineName { get; set; }
        public required string AgentPoolPath { get; set; }
        public required string AccessToken { get; set; }
        public required List<string> Tags { get; set; }
    }
}
