using Runner.Application.Commands.Identity.DTO;
using Runner.Domain.Entities.Identity;

namespace Runner.Application.Services
{
    public class IdentityProvider
    {
        public UserSafeDTO? User { get; private set; }
        public bool IsLogged { get => User != null; }

        internal void Clear()
        {
            User = null;
        }

        internal void Set(UserSafeDTO user)
        {
            User = user;
        }
    }
}
