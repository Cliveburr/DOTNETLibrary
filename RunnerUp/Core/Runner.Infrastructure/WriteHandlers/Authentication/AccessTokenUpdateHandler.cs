using Runner.Domain.Write.Authentication;
using Runner.Infrastructure.Collections;
using Runner.Kernel.Events;
using Runner.Kernel.Events.Write;

namespace Runner.Infrastructure.WriteHandlers.Identity
{
    internal class AccessTokenUpdateHandler(AccessTokenCollection accessTokenCollection)
        : IWriteHandler<AccessTokenUpdate>
    {
        public Task Handler(EventProcess process, AccessTokenUpdate request, CancellationToken cancellationToken)
        {
            return accessTokenCollection
                .UpdateAsync(request.AccessToken);
        }
    }
}
