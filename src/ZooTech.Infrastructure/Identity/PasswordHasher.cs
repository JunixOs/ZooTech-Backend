using Microsoft.AspNetCore.Identity;
using ZooTech.Application.Common.Gateway.Identity;

namespace ZooTech.Infrastructure.Identity
{
    public class PasswordHasher : IPasswordHasher
    {
        private readonly PasswordHasher<object> _hasher = new();

        public string Hash(string password)
        {
            return _hasher.HashPassword(null, password);
        }

        public bool Compare(string password, string hashedPassword)
        {
            var result = _hasher.VerifyHashedPassword(
                null,
                hashedPassword,
                password
            );

            return result != PasswordVerificationResult.Failed;
        }
    }
}