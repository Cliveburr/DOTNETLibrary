using Microsoft.Extensions.Configuration;

namespace Build.Scripts.Tests.Configuration
{
    public static class ReadConfiguraiton
    {
        public static IConfiguration Configuration()
        {
            var builder = new ConfigurationBuilder()
                .AddUserSecrets(typeof(AzureTests).Assembly);

            return builder.Build();
        }

        public static (string Uri, string Token) ReadUriAndToken()
        {
            var configuration = Configuration();
            var uri = configuration["uri"];
            var token = configuration["token"];

            return (uri, token);
        }
    }
}
