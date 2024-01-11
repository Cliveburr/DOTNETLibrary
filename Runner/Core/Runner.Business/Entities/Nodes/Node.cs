using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Entities.Nodes
{
    public class Node
    {
        [BsonId]
        public ObjectId NodeId { get; set; }
        public required string Name { get; set; }
        public NodeType Type { get; set; }
        public ObjectId? ParentId { get; set; }
    }
}
