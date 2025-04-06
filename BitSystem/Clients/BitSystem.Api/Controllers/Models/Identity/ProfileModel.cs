namespace BitSystem.Api.Controllers.Models.Profile;

public class LoginAuthenticationRequest
{
    public string? Login { get; set; }
    public string? Password { get; set; }
}

public class AuthenticationResponse
{
    public required string Token { get; set; }
    //public string? Password { get; set; }
}
