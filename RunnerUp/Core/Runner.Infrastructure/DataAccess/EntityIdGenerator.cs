using MongoDB.Bson;
using Runner.Domain.Entities;

namespace Runner.Infrastructure.DataAccess
{
    internal static class EntityIdGenerator
    {
        public static EntityId GenerateNewId()
        {
            return new EntityId
            {
                Content = ObjectId.GenerateNewId((int)DateTimeOffset.Now.Ticks).ToString()
            };
        }
    }
}
