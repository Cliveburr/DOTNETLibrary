using Runner.Domain.Write.Identity;
using Runner.Infrastructure.Collections;
using Runner.Infrastructure.DataAccess;
using Runner.Kernel.Events;
using Runner.Kernel.Events.Write;

namespace Runner.Infrastructure.WriteHandlers.Identity
{
    internal class UserInsertHandler(UserCollection userCollection)
        : IWriteHandler<UserInsert>
    {
        public Task Handler(EventProcess process, UserInsert request, CancellationToken cancellationToken)
        {
            request.User.UserId = EntityIdGenerator.GenerateNewId();
            return userCollection
                .InsertAsync(request.User);
        }
    }
}
