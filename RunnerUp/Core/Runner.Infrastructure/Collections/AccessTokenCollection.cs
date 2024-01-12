using MongoDB.Driver;
using Runner.Business.DataAccess;
using Runner.Domain.Entities.Authentication;
using Runner.Infrastructure.DataAccess;

namespace Runner.Infrastructure.Collections
{
    internal class AccessTokenCollection : CollectionBase<AccessToken>
    {
        public override string Name => "AccessToken";

        public AccessTokenCollection(MainDatabase database)
            : base(database)
        {
        }

        public override FilterDefinition<AccessToken> CreateEntityIdFilter(AccessToken doc)
        {
            return Builders<AccessToken>.Filter
                .Eq(u => u.AccessTokenId, doc.AccessTokenId);
        }
    }
}
