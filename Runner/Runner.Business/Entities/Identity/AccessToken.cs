﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Entities.Identity
{
    public class AccessToken : DocumentBase
    {
        public ObjectId UserId { get; set; }
        public required string Token { get; set; }
        public DateTime ExpireDateimeUTC { get; set; }
        public AccessTokenType Type { get; set; }
        public AccessTokenState State { get; set; }
    }
}