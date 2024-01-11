using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Entities.Nodes.Types
{
    public class App
    {
        [BsonId]
        public ObjectId AppId { get; set; }
        public ObjectId NodeId { get; set; }
        public required ObjectId OwnerId { get; set; }
    }
}
