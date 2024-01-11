using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Runner.Application.Commands.Authentication;
using Runner.Application.Services;
using Runner.Kernel.DependecyInjection;
using Runner.Kernel.Services;
using Runner.WebUI.Helpers;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace Runner.WebUI.Services.Authentication
{
    public class AuthenticationService
    {
        public const string TOKEN_STORAGE = "TOKEN";
        private const int TOKEN_EXPIRE_MONTHS = 3;
        private const int TOKEN_EXPIRE_RENEW_MONTHS = 1;
        private GlobalJavascript _js;
        private readonly KernelService _kernel;
        //private AccessTokenService _accessTokenService;
        //private readonly Business.Authentication.AuthenticationService _authenticationService;
        //public UserLogged UserLogged { get; private set; }

        public AuthenticationService(GlobalJavascript js, KernelService kernel /* Business.Authentication.AuthenticationService authenticationService, UserService userService, AccessTokenService accessTokenService, UserLogged userLogged*/)
        {
            _js = js;
            _kernel = kernel;
            //_authenticationService = authenticationService;
            //_userService = userService;
            //_accessTokenService = accessTokenService;
            //UserLogged = userLogged;
        }

        public async Task ValidateAccessToken()
        {
            var token = await _js.GetStorage(TOKEN_STORAGE);
            if (token != null)
            {
                var isValid = await _kernel.Exec(new ValidateWebUIAccessToken(token));
                if (!isValid)
                {
                    await _js.RemoveStorage(TOKEN_STORAGE);
                }
            }
        }

        //public Task Create(string? name, string? fullName, string? email, string? password, string? confirmPassword)
        //{
        //    Assert.Strings.MustNotNullOrEmpty(name, "Name é requerido");
        //    Assert.Strings.MustNotNullOrEmpty(fullName, "FullName é requerido");
        //    Assert.Strings.MustNotNullOrEmpty(email, "FullName é requerido");

        //    Assert.Strings.MustNotNullOrEmpty(password, "Password é requerido");
        //    Assert.MustTrue(password == confirmPassword, "Password precisam ser iguais");

        //    var build = BuildHashPassword(password);

        //    return _userService.Create(new User
        //    {
        //        Name = name,
        //        FullName = fullName,
        //        Email = email,
        //        PasswordHash = build.PasswordHash,
        //        PasswordSalt = build.PasswordSalt
        //    });
        //}

        public async Task Login(string? name, string? password)
        {
            Assert.Strings.MustNotNullOrEmpty(name, "Name é requerido");
            Assert.Strings.MustNotNullOrEmpty(password, "Password é requerido");

            try
            {
                var accessToken = await _kernel.Exec(new LoginByPassword(name, password));
                await _js.SetStorage(TOKEN_STORAGE, accessToken);
            }
            catch
            {
                throw new RunnerException("Password or user invalid!");
            }
        }

        //public async Task Logoff()
        //{
        //    if (UserLogged.User != null)
        //    {
        //        var accessToken = await _accessTokenService.ReadByUserAndType(UserLogged.User.UserId, AccessTokenType.WebUI);
        //        if (accessToken != null)
        //        {
        //            accessToken.State = AccessTokenState.Inactive;
        //            await _accessTokenService.SaveAsync(accessToken);
        //        }

        //        await _js.RemoveStorage(TOKEN_STORAGE);
        //        UserLogged.User = null;
        //    }
        //}

        //public (string PasswordSalt, string PasswordHash) BuildHashPassword(string password)
        //{
        //    var passwordSalt = GenerateSalt();
        //    var passwordHash = HashPassword(password, passwordSalt);
        //    return (passwordSalt, passwordHash);
        //}

        //
        //
        //private const int PASSWORD_NSALT = 256;
        //

        

        //private string GenerateSalt()
        //{
        //    var saltBytes = RandomNumberGenerator.GetBytes(PASSWORD_NSALT);
        //    return Convert.ToBase64String(saltBytes);
        //}


    }
}
