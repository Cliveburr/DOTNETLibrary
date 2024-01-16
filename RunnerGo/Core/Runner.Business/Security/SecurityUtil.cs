using System.Security.Cryptography;

namespace Runner.Business.Security
{
    internal static class SecurityUtil
    {
        public const int TOKEN_EXPIRE_MONTHS = 3;
        public const int TOKEN_EXPIRE_RENEW_MONTHS = 1;

        private const int PASSWORD_ITERATIONS = 11001;
        private const int PASSWORD_NHASH = 128;
        private const int PASSWORD_NSALT = 256;
        private const int NTOKEN = 512;

        public static string HashPassword(string password, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);

            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, PASSWORD_ITERATIONS, HashAlgorithmName.SHA512))
            {
                return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(PASSWORD_NHASH));
            }
        }

        public static string GenerateToken()
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(NTOKEN);
            return Convert.ToBase64String(tokenBytes);
        }

        public static (string PasswordSalt, string PasswordHash) BuildHashPassword(string password)
        {
            var passwordSalt = GenerateSalt();
            var passwordHash = HashPassword(password, passwordSalt);
            return (passwordSalt, passwordHash);
        }

        private static string GenerateSalt()
        {
            var saltBytes = RandomNumberGenerator.GetBytes(PASSWORD_NSALT);
            return Convert.ToBase64String(saltBytes);
        }
    }
}
