using MongoDB.Bson.Serialization.Attributes;
using Runner.Business.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Entities.Node
{
    [BsonDiscriminator("DataType")]
    public class DataType : NodeBase
    {
        public required DataTypeStruct Struct { get; set; }
    }
}
