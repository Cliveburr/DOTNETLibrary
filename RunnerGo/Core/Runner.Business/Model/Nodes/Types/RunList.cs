using MongoDB.Bson;
using Runner.Business.Entities.Nodes.Types;

namespace Runner.Business.Model.Nodes.Types
{
    public class RunList
    {
        public ObjectId RunId { get; set; }
        public RunStatus Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Completed { get; set; }
    }
}
