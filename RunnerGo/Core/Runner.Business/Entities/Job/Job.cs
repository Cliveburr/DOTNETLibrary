using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Runner.Business.Entities.Job
{
    public class Job
    {
        [BsonId]
        public ObjectId JobId { get; set; }
        public JobType Type { get; set; }
        //public required string AgentPool { get; set; }
        //public required List<string> Tags { get; set; }
        //public ObjectId? AgentId { get; set; }
        //public ObjectId RunId { get; set; }
        //public int ActionId { get; set; }
        public DateTime Queued { get; set; }
        public JobStatus Status { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime? Started { get; set; }
        public DateTime? End { get; set; }

        public ObjectId? AgentId { get; set; }
        public ObjectId? ScriptContentId { get; set; }
        public ObjectId? ScriptPackageId { get; set; }
    }

    //public class JobAgentUpdate : Job
    //{
    //    public ObjectId AgentId { get; set; }
    //}
}
