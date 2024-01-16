using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Runner.Business.Entities.Identity
{
    public class User
    {
        [BsonId]
        public ObjectId UserId { get; set; }
        public required string NickName { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public required string PasswordSalt { get; set; }
    }
}
