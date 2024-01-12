using Runner.Application.Commands.Authentication;
using Runner.Application.Services;
using Runner.Domain.Entities.Authentication;
using Runner.Domain.Read.Authentication;
using Runner.Domain.Write.Authentication;
using Runner.Kernel.Events;
using Runner.Kernel.Events.Command;

namespace Runner.Application.CommandHandlers.Authentication
{
    public class LogoffHandler(IdentityProvider identityProvider)
        : ICommandHandler<Logoff>
    {
        public async Task Handler(EventProcess process, Logoff request, CancellationToken cancellationToken)
        {
            if (identityProvider.IsLogged)
            {
                var accessToken = await process.Exec(new ReadByUserId(identityProvider.User!.UserId, AccessTokenType.WebUI));
                if (accessToken != null)
                {
                    accessToken.State = AccessTokenState.Inactive;
                    await process.Exec(new AccessTokenUpdate(accessToken));
                }
                identityProvider.Clear();
            }
        }
    }
}
