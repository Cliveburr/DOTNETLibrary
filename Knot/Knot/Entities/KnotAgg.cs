using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Knot.Entities
{
    internal class KnotAgg : Knot
    {
        [BsonElement("childs")]
        internal Knot[] ChildsAgg { get; set; }

        [BsonElement("parents")]
        internal Knot[] ParentsAgg { get; set; }
    }
}