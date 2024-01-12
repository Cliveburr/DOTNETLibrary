using MongoDB.Driver;
using Runner.Business.DataAccess;
using Runner.Domain.Entities.Identity;
using Runner.Infrastructure.DataAccess;

namespace Runner.Infrastructure.Collections
{
    internal class UserCollection : CollectionBase<User>
    {
        public override string Name => "User";

        public UserCollection(MainDatabase database)
            : base(database)
        {
        }

        public override FilterDefinition<User> CreateEntityIdFilter(User doc)
        {
            return Builders<User>.Filter
                .Eq(u => u.UserId, doc.UserId);
        }
    }
}
