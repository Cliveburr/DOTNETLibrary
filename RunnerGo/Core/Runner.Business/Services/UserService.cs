using MongoDB.Bson;
using Runner.Business.DataAccess;
using Runner.Business.Entities.Identity;
using Runner.Business.Security;

namespace Runner.Business.Services
{
    public class UserService : DataServiceBase
    {
        public UserService(Database database)
            : base(database)
        {
        }

        public Task<User?> ReadById(ObjectId userId)
        {
            return User
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public Task<User?> ReadByNickName(string nickName)
        {
            return User
                .FirstOrDefaultAsync(u => u.NickName.ToLower().Equals(nickName.ToLower()));
        }

        public Task<User?> ReadByEmail(string email)
        {
            return User
                .FirstOrDefaultAsync(u => u.Email.ToLower().Equals(email.ToLower()));
        }

        public async Task Register(string nickName, string fullName, string email, string password, string? confirmPassword)
        {
            Assert.MustTrue(password == confirmPassword, "Password precisam ser iguais");

            var hasByNickName = await ReadByNickName(nickName);
            Assert.MustNull(hasByNickName, "NickName já usado!");

            var hasByEmail = await ReadByEmail(email);
            Assert.MustNull(hasByNickName, "Email já usado!");

            var build = SecurityUtil.BuildHashPassword(password);

            await User
                .InsertAsync(new User
                {
                    NickName = nickName,
                    FullName = fullName,
                    Email = email,
                    PasswordHash = build.PasswordHash,
                    PasswordSalt = build.PasswordSalt
                });
        }
    }
}
