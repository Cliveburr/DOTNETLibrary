using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Entities
{
    public class Job : DocumentBase
    {
        public required string AgentPool { get; set; }
        public required List<string> Tags { get; set; }
        public ObjectId RunId { get; set; }
        public int ActionId { get; set; }
        public DateTime Queued { get; set; }
        public JobStatus Status { get; set; }
        public DateTime? Started { get; set; }
        public DateTime? End { get; set; }
    }
}
