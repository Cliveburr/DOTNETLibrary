using Runner.Application.Commands.Identity;
using Runner.Application.Commands.Identity.DTO;
using Runner.Kernel.Events;
using Runner.Kernel.Events.Command;

namespace Runner.Application.CommandHandlers.Identity
{
    internal class ReadByNameHandler
        : ICommandResultHandler<ReadByName, UserSafeDTO?>
    {
        public Task<UserSafeDTO?> Handler(EventProcess process, ReadByName request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
            //return process
            //    .Execute(new CheckAuthorization(Resource.User, AccessType.Read))
            //    .ThenExecute(new UserLogActivity(Resource.User, AccessType.Read))
            //    .ThenExecute(new ReadByNameQuery(request.name))
            //    .MapTo<UserSafeDTO>();
        }
    }
}
