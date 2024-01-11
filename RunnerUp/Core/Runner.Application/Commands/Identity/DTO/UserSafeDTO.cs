
namespace Runner.Application.Commands.Identity.DTO
{
    public class UserSafeDTO
    {
        public Guid UserId { get; set; }
        public required string Name { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
    }
}
