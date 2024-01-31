using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Runner.Business.DataAccess.Attributes;

namespace Runner.Business.Entities.Identity
{
    [DatabaseDef]
    public class User
    {
        [BsonId]
        public ObjectId UserId { get; set; }
        [IndexDef]
        public required string NickName { get; set; }
        [IndexDef]
        public required string FullName { get; set; }
        [IndexDef]
        public required string Email { get; set; }
        [IndexDef]
        public required string PasswordHash { get; set; }
        public required string PasswordSalt { get; set; }
    }
}
