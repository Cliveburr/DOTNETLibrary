using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Runner.Business.DataAccess.Attributes;

namespace Runner.Business.Entities.Nodes.Types
{
    [DatabaseDef]
    public class FlowSchedule
    {
        [BsonId]
        public ObjectId FlowScheduleId { get; set; }
        [IndexDef]
        public required ObjectId NodeId { get; set; }
        public required ObjectId JobScheduleId { get; set; }
    }
}
