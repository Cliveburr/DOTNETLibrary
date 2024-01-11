using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Runner.Business.Entities.Node;

namespace Runner.Business.Entities.Authentication
{
    public class AccessToken
    {
        [BsonId]
        public ObjectId AccessTokenId { get; set; }
        public ObjectId UserId { get; set; }
        public required string Token { get; set; }
        public DateTime ExpireDateimeUTC { get; set; }
        public AccessTokenType Type { get; set; }
        public AccessTokenState State { get; set; }
    }
}
