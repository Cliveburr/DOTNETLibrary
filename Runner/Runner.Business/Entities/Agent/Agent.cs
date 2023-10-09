using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Entities.Agent
{
    public class Agent
    {
        public ObjectId Id { get; set; }
        public DateTime HeartBeat { get; set; }
        public AgentStatus Status { get; set; }
    }
}
