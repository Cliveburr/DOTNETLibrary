using Runner.Business.Entities.AccessToken;
using Runner.Business.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Authentication
{
    public class AuthenticationService
    {
        public UserService UserService { get; private set; }
        public AccessTokenService AccessTokenService { get; private set; }
        public UserLogged UserLogged { get; private set; }

        public AuthenticationService(UserService userService, AccessTokenService accessTokenService, UserLogged userLogged)
        {
            UserService = userService;
            AccessTokenService = accessTokenService;
            UserLogged = userLogged;
        }

        public async Task<bool> CheckAccessToken(string token, AccessTokenType type, int renewMonths = -1)
        {
            var accessToken = await AccessTokenService.ReadByToken(token, type);
            if (accessToken != null)
            {
                if (accessToken.State == AccessTokenState.Active)
                {
                    var user = await UserService.ReadByIdAsync(accessToken.UserId);
                    if (user != null)
                    {
                        if (DateTime.UtcNow > accessToken.ExpireDateimeUTC)
                        {
                            accessToken.State = AccessTokenState.Inactive;
                            await AccessTokenService.SaveAsync(accessToken);
                        }
                        else
                        {
                            if (renewMonths > -1 && DateTime.UtcNow.AddMonths(renewMonths) > accessToken.ExpireDateimeUTC)
                            {
                                accessToken.ExpireDateimeUTC = DateTime.UtcNow.AddMonths(renewMonths);
                                await AccessTokenService.SaveAsync(accessToken);
                            }

                            UserLogged.User = user;
                            return true;
                        }
                    }
                }
            }
            UserLogged.User = null;
            return false;
        }
    }
}
