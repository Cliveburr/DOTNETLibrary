namespace BitSystem.Api.Controllers.Models.Profile;

public class RegisterRequest
{
    public required string NickName { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}
