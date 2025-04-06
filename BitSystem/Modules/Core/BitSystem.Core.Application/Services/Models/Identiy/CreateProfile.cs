
namespace BitSystem.Core.Application.Services.Models.Identiy;

public class RegisterProfile
{
    public required string NickName { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}
