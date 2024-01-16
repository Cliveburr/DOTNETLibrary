using Runner.Business.Entities.Security;
using Runner.WebUI.Helpers;

namespace Runner.WebUI.Security
{
    public class WebAuthenticationService(GlobalJavascript js, Business.Security.AuthenticationService authenticationService)
    {
        public const string TOKEN_STORAGE = "TOKEN";

        public async Task ValidateAccessToken()
        {
            var token = await js.GetStorage(TOKEN_STORAGE);
            if (token != null)
            {
                if (!(await authenticationService.LoginByAccessToken(token, AccessTokenType.WebUI)))
                {
                    await js.RemoveStorage(TOKEN_STORAGE);
                }
            }
        }

        public async Task Login(string login, string password)
        {
            var accessToken = await authenticationService.LoginByPassword(login, password, AccessTokenType.WebUI);
            await js.SetStorage(TOKEN_STORAGE, accessToken);
        }

        public async Task Logoff()
        {
            await authenticationService.Logoff();
            await js.RemoveStorage(TOKEN_STORAGE);
        }
    }
}
