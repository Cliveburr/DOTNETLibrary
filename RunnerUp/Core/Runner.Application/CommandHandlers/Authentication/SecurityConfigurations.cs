using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Application.CommandHandlers.Authentication
{
    internal static class SecurityConfigurations
    {
        public const int TOKEN_EXPIRE_MONTHS = 3;
        public const int TOKEN_EXPIRE_RENEW_MONTHS = 1;

        public const int PASSWORD_ITERATIONS = 11001;
        public const int PASSWORD_NHASH = 128;
        public const int NTOKEN = 512;
    }
}
