using MongoDB.Bson;
using MongoDB.Driver;
using Runner.Business.DataAccess;
using Runner.Business.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Services
{
    public class AccessTokenService : ServiceBase
    {
        public AccessTokenService(Database database)
            : base(database)
        {
        }

        public Task<AccessToken?> ReadByUserId(ObjectId userId)
        {
            return AccessToken
                .FirstOrDefaultAsync(at => at.UserId == userId);
        }

        public Task<AccessToken?> ReadByToken(string token)
        {
            return AccessToken
                .FirstOrDefaultAsync(at => at.Token == token);
        }

        public Task<AccessToken?> ReadByUserAndType(ObjectId userId, AccessTokenType type)
        {
            return AccessToken
                .FirstOrDefaultAsync(at => at.UserId == userId && at.Type == type);
        }

        public Task CreateAsync(AccessToken accessToken)
        {
            // checar se o name é unico
            // checar se já existe email

            return AccessToken.CreateAsync(accessToken);
        }

        public Task SaveAsync(AccessToken accessToken)
        {
            return AccessToken
                .SaveAsync(accessToken);
        }
    }
}
