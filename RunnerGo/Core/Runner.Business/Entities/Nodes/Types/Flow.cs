using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Runner.Business.DataAccess.Attributes;
using Runner.Business.Actions;
using Runner.Business.Datas.Model;

namespace Runner.Business.Entities.Nodes.Types
{
    [DatabaseDef]
    public class Flow
    {
        [BsonId]
        public ObjectId FlowId { get; set; }
        [IndexDef]
        public required ObjectId NodeId { get; set; }
        public required FlowAction Root { get; set; }
        public required List<DataProperty> Input { get; set; }
    }

    public class FlowAction
    {
        public required string Label { get; set; }
        public ActionType Type { get; set; }
        public List<FlowAction>? Childs { get; set; }
        public List<DataProperty>? Data { get; set; }
    }
}
