using Runner.Application.Commands.Identity;
using Runner.Application.Commands.Nodes.Types;
using Runner.Application.Commands.Nodes.Types.DTO;
using Runner.Application.Security;
using Runner.Application.Services;
using Runner.Domain.Entities.Identity;
using Runner.Domain.Read.Nodes.Types;
using Runner.Domain.Write.Identity;
using Runner.Kernel.Events;
using Runner.Kernel.Events.Command;

namespace Runner.Application.CommandHandlers.Nodes.Types
{
    internal class ReadLoggedHandler(IdentityProvider identityProvider)
        : ICommandResultHandler<ReadLogged, List<AppDTO>>
    {
        public async Task<List<AppDTO>> Handler(EventProcess process, ReadLogged request, CancellationToken cancellationToken)
        {
            Assert.MustNotNull(identityProvider.User, "Not logged!");

            // checar permissão


            // ler apps
            var apps = await process.Exec(new ReadByOwner(identityProvider.User.UserId));

            // ler nodes
            var nodes = await 

            var securityUtil = new SecurityUtil();
            var build = securityUtil.BuildHashPassword(request.Password);

            return process.Exec(new UserInsert(new User
            {
                Name = request.Name,
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = build.PasswordHash,
                PasswordSalt = build.PasswordSalt
            }));


            //return process
            //    .Execute(new CheckAuthorization(Resource.User, AccessType.Read))
            //    .ThenExecute(new UserLogActivity(Resource.User, AccessType.Read))
            //    .ThenExecute(new ReadByNameQuery(request.name))
            //    .MapTo<UserSafeDTO>();
        }
    }
}
