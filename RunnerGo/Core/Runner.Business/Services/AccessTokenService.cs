using MongoDB.Bson;
using Runner.Business.DataAccess;
using Runner.Business.Entities.Security;

namespace Runner.Business.Services
{
    public class AccessTokenService : DataServiceBase
    {
        public AccessTokenService(Database database)
            : base(database)
        {
        }

        public Task<AccessToken?> ReadByToken(string token, AccessTokenType type)
        {
            return AccessToken
                .FirstOrDefaultAsync(at => at.Token == token && at.Type == type);
        }

        public Task<AccessToken?> ReadByUserAndType(ObjectId userId, AccessTokenType type)
        {
            return AccessToken
                .FirstOrDefaultAsync(at => at.UserId == userId && at.Type == type);
        }

        public Task Update(AccessToken accessToken)
        {
            return AccessToken
                .ReplaceAsync(at => at.AccessTokenId == accessToken.AccessTokenId, accessToken);
        }

        public Task Insert(AccessToken accessToken)
        {
            return AccessToken
                .InsertAsync(accessToken);
        }
    }
}
