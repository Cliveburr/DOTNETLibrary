using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using Runner.Domain.Entities;
using System.Runtime.Serialization;

namespace Runner.Infrastructure.DataAccess.Serializer
{
    public class EntityIdSerializer : SerializerBase<EntityId>
    {
        public override EntityId Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var type = context.Reader.GetCurrentBsonType();
            if (type == BsonType.ObjectId)
            {
                var objectId = context.Reader.ReadObjectId();
                if (objectId == ObjectId.Empty)
                {
                    return new EntityId
                    {
                        Content = null
                    };
                }
                else
                {
                    return new EntityId
                    {
                        Content = objectId.ToString()
                    };
                }
            }
            else
            {
                throw new SerializationException("Invalid ObjectId deserialization!");
            }
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, EntityId value)
        {
            if (string.IsNullOrEmpty(value.Content))
            {
                context.Writer.WriteObjectId(ObjectId.Empty);
            }
            else
            {
                if (ObjectId.TryParse(value.Content, out var id))
                {
                    context.Writer.WriteObjectId(id);
                }
                else
                {
                    throw new SerializationException("Invalid ObjectId format! " + value.Content);
                }
            }
        }
    }
}
