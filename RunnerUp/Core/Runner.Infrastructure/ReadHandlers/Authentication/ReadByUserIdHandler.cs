using Runner.Domain.Entities.Authentication;
using Runner.Domain.Read.Authentication;
using Runner.Infrastructure.Collections;
using Runner.Kernel.Events;
using Runner.Kernel.Events.Read;

namespace Runner.Infrastructure.ReadHandlers.Authentication
{
    internal class ReadByUserIdHandler(AccessTokenCollection accessTokenCollection)
        : IReadHandler<ReadByUserId, AccessToken?>
    {
        public Task<AccessToken?> Handler(EventProcess process, ReadByUserId request, CancellationToken cancellationToken)
        {
            return accessTokenCollection
                .FirstOrDefaultAsync(at => at.UserId == request.UserId && at.Type == request.Type);
        }
    }
}
