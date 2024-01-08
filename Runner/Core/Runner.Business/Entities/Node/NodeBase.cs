using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Entities.Node
{
    public abstract class DocumentBase
    {
        [BsonId]
        public ObjectId Id { get; set; }
    }

    public abstract class NodeBase : DocumentBase
    {
        public required string Name { get; set; }
        public NodeType Type { get; set; }
        public ObjectId? Parent { get; set; }
    }
}
