using Runner.Domain.Entities.Identity;
using Runner.Domain.Read.Identity;
using Runner.Infrastructure.Collections;
using Runner.Kernel.Events;
using Runner.Kernel.Events.Read;

namespace Runner.Infrastructure.QueryHandlers.Identity
{
    internal class ReadByNameHandler(UserCollection userCollection)
        : IReadHandler<ReadByName, User?>
    {
        public Task<User?> Handler(EventProcess process, ReadByName request, CancellationToken cancellationToken)
        {

            return userCollection
                .FirstOrDefaultAsync(u => u.Name.ToLower().Equals(request.Name.ToLower()));
        }
    }
}
