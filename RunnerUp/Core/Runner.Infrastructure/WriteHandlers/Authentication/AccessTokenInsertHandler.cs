using Runner.Domain.Write.Authentication;
using Runner.Infrastructure.Collections;
using Runner.Infrastructure.DataAccess;
using Runner.Kernel.Events;
using Runner.Kernel.Events.Write;

namespace Runner.Infrastructure.WriteHandlers.Identity
{
    internal class AccessTokenInsertHandler(AccessTokenCollection accessTokenCollection)
        : IWriteHandler<AccessTokenInsert>
    {
        public Task Handler(EventProcess process, AccessTokenInsert request, CancellationToken cancellationToken)
        {
            request.AccessToken.AccessTokenId = EntityIdGenerator.GenerateNewId();
            return accessTokenCollection
                .InsertAsync(request.AccessToken);
        }
    }
}
