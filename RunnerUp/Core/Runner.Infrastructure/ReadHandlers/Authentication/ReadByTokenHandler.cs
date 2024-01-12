using Runner.Domain.Entities.Authentication;
using Runner.Domain.Read.Authentication;
using Runner.Infrastructure.Collections;
using Runner.Kernel.Events;
using Runner.Kernel.Events.Read;

namespace Runner.Infrastructure.ReadHandlers.Authentication
{
    internal class ReadByTokenHandler(AccessTokenCollection accessTokenCollection)
        : IReadHandler<ReadByToken, AccessToken?>
    {
        public Task<AccessToken?> Handler(EventProcess process, ReadByToken request, CancellationToken cancellationToken)
        {
            return accessTokenCollection
                .FirstOrDefaultAsync(at => at.Token == request.Token && at.Type == request.Type);
        }
    }
}
