using BitSystem.Api.Authentication;
using BitSystem.Api.Controllers.Models.Profile;
using BitSystem.Core.Application.Services;
using BitSystem.Core.Application.Services.Models.Identiy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BitSystem.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class IdentityController : Controller
{
    private readonly TokenGenerator _tokenGenerator;
    private readonly IdentityService _identityService;

    public IdentityController(TokenGenerator tokenGenerator, IdentityService identityService)
    {
        _tokenGenerator = tokenGenerator;
        _identityService = identityService;
    }

    [HttpPost]
    public async Task Register(RegisterRequest req)
    {
        await _identityService.Register(new RegisterProfile
        {
            NickName = req.NickName,
            FullName = req.FullName,
            Email = req.Email,
            Password = req.Password
        });
    }

    [HttpPost("LoginAuthentication")]
    public AuthenticationResponse LoginAuthentication(LoginAuthenticationRequest req)
    {
        var userId = "ttt";

        var token = _tokenGenerator.GenerateToken(userId);

        return new AuthenticationResponse
        {
            Token = token,
        };
    }

    [HttpGet("TestSafe")]
    [Authorize]
    public string TestSafe()
    {
        return "Ok";
    }
}
