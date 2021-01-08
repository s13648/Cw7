using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Cw7.Helper
{
    public static class PasswordHashHelper
    {
        public static string Create(string rawPassword, string salt)
        {
            var passwordBytes = KeyDerivation
                .Pbkdf2(rawPassword
                    , Encoding.UTF8.GetBytes(salt)
                    , KeyDerivationPrf.HMACSHA512
                    , 10000
                    ,256 / 8);

            return Convert.ToBase64String(passwordBytes);
        }

        public static bool Validate(string hash, string salt, string rawPassword) => Create(rawPassword, salt) == hash;

        public static string CreateSalt()
        {
            var randomBytes = new byte[128 / 8];
            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}
