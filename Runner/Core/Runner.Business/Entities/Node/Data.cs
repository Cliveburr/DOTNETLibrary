﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Runner.Business.Data.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Entities.Node
{
    [BsonDiscriminator("Data")]
    public class Data : Node
    {
        public string? DataTypePath { get; set; }
        public ObjectId? DataType { get; set; }
        public required DataStruct Struct { get; set; }
    }
}
