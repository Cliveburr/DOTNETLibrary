using Runner.Domain.Entities.Identity;
using Runner.Domain.Read.Identity;
using Runner.Infrastructure.Collections;
using Runner.Kernel.Events;
using Runner.Kernel.Events.Read;

namespace Runner.Infrastructure.ReadHandlers.Identity
{
    internal class ReadByIdHandler(UserCollection userCollection)
        : IReadHandler<ReadById, User?>
    {
        public Task<User?> Handler(EventProcess process, ReadById request, CancellationToken cancellationToken)
        {
            return userCollection
                .FirstOrDefaultAsync(u => u.UserId == request.UserId);
        }
    }
}
