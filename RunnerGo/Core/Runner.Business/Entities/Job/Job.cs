using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Runner.Business.DataAccess.Attributes;
using Runner.Business.Datas.Model;

namespace Runner.Business.Entities.Job
{
    [DatabaseDef]
    public class Job
    {
        [BsonId]
        public ObjectId JobId { get; set; }
        public JobType Type { get; set; }
        public DateTime Queued { get; set; }
        [IndexDef]
        public JobStatus Status { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime? Started { get; set; }
        public DateTime? End { get; set; }

        [IndexDef]
        public ObjectId? AgentId { get; set; }
        public ObjectId? ScriptContentId { get; set; }
        public ObjectId? ScriptPackageId { get; set; }

        public ObjectId? RunId { get; set; }
        public int? ActionId { get; set; }

        public ObjectId? FlowId { get; set; }
        public List<DataProperty>? RunInput { get; set; }
    }
}
