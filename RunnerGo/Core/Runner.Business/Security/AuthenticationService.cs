using MongoDB.Bson;
using Runner.Business.Entities.Identity;
using Runner.Business.Entities.Security;
using Runner.Business.Services;

namespace Runner.Business.Security
{
    public class AuthenticationService(UserService userService, AccessTokenService accessTokenService, IdentityProvider identityProvider)
    {
        public void LoginForInternalServices()
        {
            identityProvider.Set(new User
            {
                UserId = ObjectId.Empty,
                NickName = "Internal Services",
                FullName = "Internal Services",
                Email = "internal@internal.com",
                PasswordHash = "",
                PasswordSalt = ""
            });
        }

        public async Task<bool> LoginByAccessToken(string token, AccessTokenType type)
        {
            var accessToken = await accessTokenService.ReadByToken(token, type);
            if (accessToken != null)
            {
                if (accessToken.State == AccessTokenState.Active)
                {
                    var user = await userService.ReadById(accessToken.UserId);
                    if (user != null)
                    {
                        if (DateTime.UtcNow > accessToken.ExpireDateimeUTC)
                        {
                            accessToken.State = AccessTokenState.Inactive;
                            await accessTokenService.Update(accessToken);
                        }
                        else
                        {
                            if (SecurityUtil.TOKEN_EXPIRE_RENEW_MONTHS > -1 && DateTime.UtcNow.AddMonths(SecurityUtil.TOKEN_EXPIRE_RENEW_MONTHS) > accessToken.ExpireDateimeUTC)
                            {
                                accessToken.ExpireDateimeUTC = DateTime.UtcNow.AddMonths(SecurityUtil.TOKEN_EXPIRE_RENEW_MONTHS);
                                await accessTokenService.Update(accessToken);
                            }

                            identityProvider.Set(user);
                            return true;
                        }
                    }
                }
            }
            identityProvider.Clear();
            return false;
        }

        public async Task<string> LoginByPassword(string login, string password, AccessTokenType webUI)
        {
            var genericErrorMessage = "Password or user invalido!";

            var user = await userService.ReadByNickName(login);
            if (user == null)
            {
                user = await userService.ReadByEmail(login);
            }
            Assert.MustNotNull(user, genericErrorMessage);

            var passwordHash = SecurityUtil.HashPassword(password, user.PasswordSalt);
            Assert.MustTrue(passwordHash.Equals(user.PasswordHash), genericErrorMessage);

            var accessToken = await accessTokenService.ReadByUserAndType(user.UserId, AccessTokenType.WebUI);
            if (accessToken != null)
            {
                accessToken.State = AccessTokenState.Active;
                accessToken.ExpireDateimeUTC = DateTime.UtcNow.AddMonths(SecurityUtil.TOKEN_EXPIRE_MONTHS);
                accessToken.Token = SecurityUtil.GenerateToken();
                await accessTokenService.Update(accessToken);
            }
            else
            {
                accessToken = new AccessToken
                {
                    UserId = user.UserId,
                    Token = SecurityUtil.GenerateToken(),
                    ExpireDateimeUTC = DateTime.UtcNow.AddMonths(SecurityUtil.TOKEN_EXPIRE_MONTHS),
                    Type = AccessTokenType.WebUI,
                    State = AccessTokenState.Active
                };
                await accessTokenService.Insert(accessToken);
            }

            identityProvider.Set(user);
            
            return accessToken.Token;
        }

        public async Task Logoff()
        {
            if (identityProvider.User != null)
            {
                var accessToken = await accessTokenService.ReadByUserAndType(identityProvider.User.UserId, AccessTokenType.WebUI);
                if (accessToken != null)
                {
                    accessToken.State = AccessTokenState.Inactive;
                    await accessTokenService.Update(accessToken);
                }
                identityProvider.Clear();
            }
        }
    }
}
