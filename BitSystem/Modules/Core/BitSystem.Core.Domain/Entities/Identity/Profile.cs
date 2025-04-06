using BitSystem.Shared.Domain;

namespace BitSystem.Core.Domain.Entities.Identity;

public sealed class Profile
{
    public EntityId ProfileId { get; set; }
    public required string NickName { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string PasswordSalt { get; set; }
}
