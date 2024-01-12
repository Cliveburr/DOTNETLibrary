using Runner.Kernel.Events.Command;
using Runner.Kernel.Events;
using Runner.Application.Commands.Identity;
using Runner.Domain.Entities.Identity;
using Runner.Application.Security;
using Runner.Domain.Write.Identity;

namespace Runner.Application.CommandHandlers.Identity
{
    internal class CreateHandlerHandler
        : ICommandHandler<CreateUser>
    {
        public Task Handler(EventProcess process, CreateUser request, CancellationToken cancellationToken)
        {
            Assert.Strings.MustNotNullOrEmpty(request.Name, "Name é requerido");
            Assert.Strings.MustNotNullOrEmpty(request.FullName, "FullName é requerido");
            Assert.Strings.MustNotNullOrEmpty(request.Email, "FullName é requerido");

            Assert.Strings.MustNotNullOrEmpty(request.Password, "Password é requerido");
            Assert.MustTrue(request.Password == request.ConfirmPassword, "Password precisam ser iguais");

            //TODO: checar se já existe o email

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
