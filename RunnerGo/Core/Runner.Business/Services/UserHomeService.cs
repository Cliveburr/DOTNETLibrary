using MongoDB.Driver;
using Runner.Business.DataAccess;
using Runner.Business.Entities.Identity;
using Runner.Business.Security;

namespace Runner.Business.Services
{
    public class UserHomeService : DataServiceBase
    {
        public IdentityProvider IdentityProvider { get; init; }

        public UserHomeService(Database database, IdentityProvider identityProvider)
            : base(database)
        {
            IdentityProvider = identityProvider;
        }

        public async Task<UserHome> ReadOrCreate()
        {
            Assert.MustNotNull(IdentityProvider.User, "Not logged!");

            var userHome = await UserHome
                .FirstOrDefaultAsync(uh => uh.UserId == IdentityProvider.User.UserId);
            if (userHome is not null)
            {
                return userHome;
            }
            else
            {
                userHome = new UserHome
                {
                    UserId = IdentityProvider.User.UserId,
                    Favorite = new List<UserHomeFavorite>()
                };
                await UserHome
                    .InsertAsync(userHome);
                return userHome;
            }
        }

        public Task AddFavorite(UserHomeFavorite favorite)
        {
            Assert.MustNotNull(IdentityProvider.User, "Not logged!");

            var update = Builders<UserHome>.Update
                .Push(r => r.Favorite, favorite);

            return UserHome 
                .UpdateAsync(uh => uh.UserId == IdentityProvider.User.UserId, update);
        }

        public Task UpdateFavorite(List<UserHomeFavorite> favorites)
        {
            Assert.MustNotNull(IdentityProvider.User, "Not logged!");

            var update = Builders<UserHome>.Update
                .Set(r => r.Favorite, favorites);

            return UserHome
                .UpdateAsync(uh => uh.UserId == IdentityProvider.User.UserId, update);
        }
    }
}
