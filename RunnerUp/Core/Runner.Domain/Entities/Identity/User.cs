
namespace Runner.Domain.Entities.Identity
{
    public sealed class User
    {
        public EntityId UserId { get; set; }
        public required string Name { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public required string PasswordSalt { get; set; }
    }
}
