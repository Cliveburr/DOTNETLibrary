using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Runner.Business.Authentication;
using Runner.Business.Entities;
using Runner.Business.Entities.AccessToken;
using Runner.Business.Entities.User;
using Runner.Business.Services;
using Runner.WebUI.Helpers;
using System.Security.Cryptography;

namespace Runner.WebUI.Authentication
{
    public class WebAuthenticationService
    {
        public const string TOKEN_STORAGE = "TOKEN";
        private const int TOKEN_EXPIRE_MONTHS = 3;
        private const int TOKEN_EXPIRE_RENEW_MONTHS = 1;
        private GlobalJavascript _js;
        private UserService _userService;
        private AccessTokenService _accessTokenService;
        private readonly Business.Authentication.AuthenticationService _authenticationService;
        public UserLogged UserLogged { get; private set; }

        public bool IsLogged { get => UserLogged.User != null; }

        public WebAuthenticationService(GlobalJavascript js, Business.Authentication.AuthenticationService authenticationService, UserService userService, AccessTokenService accessTokenService, UserLogged userLogged)
        {
            _js = js;
            _authenticationService = authenticationService;
            _userService = userService;
            _accessTokenService = accessTokenService;
            UserLogged = userLogged;
        }

        public async Task CheckAccessToken()
        {
            var token = await _js.GetStorage(TOKEN_STORAGE);
            if (token != null)
            {
                if (!(await _authenticationService.CheckAccessToken(token, AccessTokenType.WebUI)))
                {
                    await _js.RemoveStorage(TOKEN_STORAGE);
                }
            }
        }

        public Task Create(string? name, string? fullName, string? email, string? password, string? confirmPassword)
        {
            Assert.Strings.MustNotNullOrEmpty(name, "Name é requerido");
            Assert.Strings.MustNotNullOrEmpty(fullName, "FullName é requerido");
            Assert.Strings.MustNotNullOrEmpty(email, "FullName é requerido");

            Assert.Strings.MustNotNullOrEmpty(password, "Password é requerido");
            Assert.MustTrue(password == confirmPassword, "Password precisam ser iguais");

            var build = BuildHashPassword(password);

            return _userService.Create(new User
            {
                Name = name,
                FullName = fullName,
                Email = email,
                PasswordHash = build.PasswordHash,
                PasswordSalt = build.PasswordSalt
            });
        }

        public async Task Login(string? name, string? password)
        {
            var genericErrorMessage = "Password or user invalido!";
            Assert.Strings.MustNotNullOrEmpty(name, "Name é requerido");
            Assert.Strings.MustNotNullOrEmpty(password, "Password é requerido");
            
            var user = await _userService.ReadByName(name);
            Assert.MustNotNull(user, genericErrorMessage);

            var passwordHash = HashPassword(password, user.PasswordSalt);

            Assert.MustTrue(passwordHash.Equals(user.PasswordHash), genericErrorMessage);

            var accessToken = await _accessTokenService.ReadByUserAndType(user.Id, AccessTokenType.WebUI);
            if (accessToken != null)
            {
                accessToken.State = AccessTokenState.Active;
                accessToken.ExpireDateimeUTC = DateTime.UtcNow.AddMonths(TOKEN_EXPIRE_MONTHS);
                accessToken.Token = GenerateToken();
                await _accessTokenService.SaveAsync(accessToken);
            }
            else
            {
                accessToken = new AccessToken
                {
                    UserId = user.Id,
                    Token = GenerateToken(),
                    ExpireDateimeUTC = DateTime.UtcNow.AddMonths(TOKEN_EXPIRE_MONTHS),
                    Type = AccessTokenType.WebUI,
                    State = AccessTokenState.Active
                };
                await _accessTokenService.CreateAsync(accessToken);
            }

            await _js.SetStorage(TOKEN_STORAGE, accessToken.Token);
            UserLogged.User = user;
        }

        public async Task Logoff()
        {
            if (UserLogged.User != null)
            {
                var accessToken = await _accessTokenService.ReadByUserAndType(UserLogged.User.Id, AccessTokenType.WebUI);
                if (accessToken != null)
                {
                    accessToken.State = AccessTokenState.Inactive;
                    await _accessTokenService.SaveAsync(accessToken);
                }

                await _js.RemoveStorage(TOKEN_STORAGE);
                UserLogged.User = null;
            }
        }

        public (string PasswordSalt, string PasswordHash) BuildHashPassword(string password)
        {
            var passwordSalt = GenerateSalt();
            var passwordHash = HashPassword(password, passwordSalt);
            return (passwordSalt, passwordHash);
        }

        private const int PASSWORD_ITERATIONS = 11001;
        private const int PASSWORD_NHASH = 128;
        private const int PASSWORD_NSALT = 256;
        private const int NTOKEN = 512;

        private string HashPassword(string password, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);

            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, PASSWORD_ITERATIONS, HashAlgorithmName.SHA512))
            {
                return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(PASSWORD_NHASH));
            }
        }

        private string GenerateSalt()
        {
            var saltBytes = RandomNumberGenerator.GetBytes(PASSWORD_NSALT);
            return Convert.ToBase64String(saltBytes);
        }

        private string GenerateToken()
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(NTOKEN);
            return Convert.ToBase64String(tokenBytes);
        }
    }
}
