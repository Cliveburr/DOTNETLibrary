using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Entities.Agent
{
    public class Job : DocumentBase
    {
        public ObjectId AgentPoolId { get; set; }
        public ObjectId AgentId { get; set; }
        public DateTime Queued { get; set; }
        public bool ToRun { get; set; }
        public DateTime? Started { get; set; }
        public DateTime? End { get; set; }
    }
}
