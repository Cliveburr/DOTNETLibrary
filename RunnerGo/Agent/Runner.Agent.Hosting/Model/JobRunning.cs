using MongoDB.Bson;

namespace Runner.Agent.Hosting.Model
{
    public class JobRunning
    {
        public required ObjectId JobId { get; set; }
        public required ObjectId RunId { get; set; }
        public required int ActionId { get; set; }
    }
}
