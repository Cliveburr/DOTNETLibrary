using Runner.Application.Commands.Authentication;
using Runner.Domain.Entities.Authentication;
using Runner.Kernel.Events.Command;
using Runner.Kernel.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Runner.Domain.Read.Identity;
using Runner.Domain.Read.Authentication;
using Runner.Domain.Write.Authentication;
using System.Security.Cryptography;
using Runner.Application.Commands.Identity.DTO;
using Runner.Application.Services;
using Runner.Application.Security;

namespace Runner.Application.CommandHandlers.Authentication
{
    public class LoginByPasswordHandler
        : ICommandResultHandler<LoginByPassword, string>
    {
        private readonly IdentityProvider _identityProvider;

        public LoginByPasswordHandler(IdentityProvider identityProvider)
        {
            _identityProvider = identityProvider;
        }

        public async Task<string> Handler(EventProcess process, LoginByPassword request, CancellationToken cancellationToken)
        {
            var user = await process.Exec(new ReadByName(request.Name));
            Assert.MustNotNull(user, "");

            var securityUtil = new SecurityUtil();
            var passwordHash = securityUtil.HashPassword(request.Password, user.PasswordSalt);
            Assert.MustTrue(passwordHash.Equals(user.PasswordHash), "");

            var accessToken = await process.Exec(new ReadByUserId(user.UserId, AccessTokenType.WebUI));
            if (accessToken != null)
            {
                accessToken.State = AccessTokenState.Active;
                accessToken.ExpireDateimeUTC = DateTime.UtcNow.AddMonths(SecurityUtil.TOKEN_EXPIRE_MONTHS);
                accessToken.Token = securityUtil.GenerateToken();
                await process.Exec(new AccessTokenUpdate(accessToken));
            }
            else
            {
                accessToken = new AccessToken
                {
                    UserId = user.UserId,
                    Token = securityUtil.GenerateToken(),
                    ExpireDateimeUTC = DateTime.UtcNow.AddMonths(SecurityUtil.TOKEN_EXPIRE_MONTHS),
                    Type = AccessTokenType.WebUI,
                    State = AccessTokenState.Active
                };
                await process.Exec(new AccessTokenInsert(accessToken));
            }

            _identityProvider.Set(process.MapTo<UserSafeDTO>(user));

            return accessToken.Token;
        }

        
    }
}
