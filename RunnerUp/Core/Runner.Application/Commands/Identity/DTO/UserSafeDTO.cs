using Runner.Domain.Entities;

namespace Runner.Application.Commands.Identity.DTO
{
    public class UserSafeDTO
    {
        public EntityId UserId { get; set; }
        public required string Name { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
    }
}
