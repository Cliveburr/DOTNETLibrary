using Runner.Kernel.Events.Command;
using Runner.Kernel.Events;
using Runner.Application.Commands.Authentication;
using Runner.Domain.Entities.Authentication;

namespace Runner.Application.CommandHandlers.Authentication
{
    internal class ValidateWebUIAccessTokenHandler
        : ICommandResultHandler<ValidateWebUIAccessToken, bool>
    {
        public Task<bool> Handler(EventProcess process, ValidateWebUIAccessToken request, CancellationToken cancellationToken)
        {
            return process.Exec(new ValidateAccessToken(request.Token, AccessTokenType.WebUI));
        }
    }
}
