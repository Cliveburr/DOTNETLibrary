using Runner.Application.Commands.Authentication;
using Runner.Kernel.Services;
using Runner.WebUI.Helpers;
using System.Xml.Linq;

namespace Runner.WebUI.Services.Authentication
{
    public class AuthenticationService
    {
        private const string TOKEN_STORAGE = "TOKEN";
        private GlobalJavascript _js;
        private readonly KernelService _kernel;

        public AuthenticationService(GlobalJavascript js, KernelService kernel)
        {
            _js = js;
            _kernel = kernel;
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

        public async Task Logoff()
        {
            await _kernel.Exec(new Logoff());
            await _js.RemoveStorage(TOKEN_STORAGE);
        }
    }
}
