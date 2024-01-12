using Runner.Kernel.Events.Command;
using Runner.Kernel.Events;
using Runner.Application.Commands.Authentication;
using Runner.Domain.Entities.Authentication;
using Runner.Domain.Read.Authentication;
using Runner.Domain.Read.Identity;
using Runner.Domain.Write.Authentication;
using Runner.Application.Services;
using Runner.Application.Commands.Identity.DTO;
using Runner.Domain.Entities.Identity;
using Runner.Application.Security;

namespace Runner.Application.CommandHandlers.Authentication
{
    internal class ValidateAccessTokenHandler
        : ICommandResultHandler<ValidateAccessToken, bool>
    {
        private readonly IdentityProvider _identityProvider;

        public ValidateAccessTokenHandler(IdentityProvider identityProvider)
        {
            _identityProvider = identityProvider;
        }

        public async Task<bool> Handler(EventProcess process, ValidateAccessToken request, CancellationToken cancellationToken)
        {
            var accessToken = await process.Exec(new ReadByToken(request.Token, request.Type));
            if (accessToken != null)
            {
                if (accessToken.State == AccessTokenState.Active)
                {
                    var user = await process.Exec(new ReadById(accessToken.UserId));
                    if (user != null)
                    {
                        if (DateTime.UtcNow > accessToken.ExpireDateimeUTC)
                        {
                            accessToken.State = AccessTokenState.Inactive;
                            await process.Exec(new AccessTokenUpdate(accessToken));
                        }
                        else
                        {
                            if (SecurityUtil.TOKEN_EXPIRE_RENEW_MONTHS > -1 && DateTime.UtcNow.AddMonths(SecurityUtil.TOKEN_EXPIRE_RENEW_MONTHS) > accessToken.ExpireDateimeUTC)
                            {
                                accessToken.ExpireDateimeUTC = DateTime.UtcNow.AddMonths(SecurityUtil.TOKEN_EXPIRE_MONTHS);
                                await process.Exec(new AccessTokenUpdate(accessToken));
                            }

                            _identityProvider.Set(process.MapTo<UserSafeDTO>(user));
                            return true;
                        }
                    }
                }
            }
            _identityProvider.Clear();
            return false;
        }
    }
}
