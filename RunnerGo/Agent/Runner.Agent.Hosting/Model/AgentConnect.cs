using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using Runner.Agent.Hosting.Hubs;

namespace Runner.Agent.Hosting.Model
{
    public class AgentConnect
    {
        public required string ConnectionId { get; set; }
        public required AgentHub Hub { get; set; }
        public required string AgentPoolPath { get; set; }
        public ObjectId AgentId { get; set; }
        public required IServiceScope Scope { get; set; }
        public JobRunning? JobRunning { get; set; }
    }
}
