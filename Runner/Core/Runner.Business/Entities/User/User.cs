using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Runner.Business.Entities.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Entities.User
{
    public class User
    {
        [BsonId]
        public ObjectId UserId { get; set; }
        public required string Name { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public required string PasswordSalt { get; set; }
    }
}
