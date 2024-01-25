
namespace Runner.Agent.Interface.Model
{
    public class UpdateVersionRequest
    {
        public int Version { get; set; }
        public required byte[] Content { get; set; }
    }
}
