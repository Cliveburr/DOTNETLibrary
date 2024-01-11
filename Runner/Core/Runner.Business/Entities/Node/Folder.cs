using MongoDB.Bson.Serialization.Attributes;
using Runner.Business.Entities.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Entities
{
    [BsonDiscriminator("Folder")]
    public class Folder : Node.Node
    {
    }
}
