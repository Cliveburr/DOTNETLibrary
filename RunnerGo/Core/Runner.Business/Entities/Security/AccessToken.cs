using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Runner.Business.DataAccess.Attributes;

namespace Runner.Business.Entities.Security
{
    [DatabaseDef]
    public class AccessToken
    {
        [BsonId]
        public ObjectId AccessTokenId { get; set; }
        public required ObjectId UserId { get; set; }
        public required string Token { get; set; }
        public DateTime ExpireDateimeUTC { get; set; }
        public AccessTokenType Type { get; set; }
        public AccessTokenState State { get; set; }
    }
}
