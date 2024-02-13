using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Runner.Business.Entities.Nodes;
using Runner.Business.DataAccess.Attributes;

namespace Runner.Business.Entities.Identity
{
    [DatabaseDef]
    public class UserHome
    {
        [BsonId]
        public ObjectId UserHomeId { get; set; }
        [IndexDef]
        public required ObjectId UserId { get; set; }
        public required List<UserHomeFavorite> Favorite { get; set; }
    }

    public class UserHomeFavorite
    {
        public required string Title { get; set; }
        public required string Subtitle { get; set; }
        public string? Icon { get; set; }
        public NodeType? NodeType { get; set; }
        public ObjectId? NodeId { get; set; }
        public string? DirectPath { get; set; }
    }
}
