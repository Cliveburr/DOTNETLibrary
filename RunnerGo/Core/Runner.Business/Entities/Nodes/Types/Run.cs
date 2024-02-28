using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Runner.Business.DataAccess.Attributes;
using Runner.Business.Datas.Model;

namespace Runner.Business.Entities.Nodes.Types
{
    [DatabaseDef]
    public class Run
    {
        [BsonId]
        public ObjectId RunId { get; set; }
        [IndexDef]
        public required ObjectId FlowId { get; set; }
        public required List<Actions.Action> Actions { get; set; }
        public int RootActionId { get; set; }
        public int IdIndexes { get; set; }
        public RunStatus Status { get; set; }
        [IndexDef]
        public DateTime Created { get; set; }
        public DateTime? Completed { get; set; }
        public required List<RunLog> Log { get; set; }
        public List<DataProperty>? Input { get; set; }
        public FromParentRun? FromParentRun { get; set; }
    }

    public enum RunStatus
    {
        Waiting = 0,
        Running = 1,
        Error = 2,
        Completed = 3
    }

    public class RunLog
    {
        public DateTime Created { get; set; }
        public required string Text { get; set; }
        public string? FullError { get; set; }
    }

    public class FromParentRun
    {
        public required ObjectId RunId { get; set; }
        public required int ActionId { get; set; }
    }
}
