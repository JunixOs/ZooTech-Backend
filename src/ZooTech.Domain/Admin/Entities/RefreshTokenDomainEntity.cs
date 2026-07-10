namespace ZooTech.Domain.Admin.Entities
{
    public class RefreshTokenDomainEntity
    {
        public int Id { get; private set; }
        public int AdminUserId { get; private set; }
        public string Token { get; private set; } = default!;
        public string? JwtId { get; private set; }
        public bool IsRevoked { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? RevokedAt { get; private set; }

        public RefreshTokenDomainEntity() { }

        public RefreshTokenDomainEntity Create(
            int id,
            int adminUserId,
            string token,
            string? jwtId,
            bool isRevoked,
            DateTime expiresAt,
            DateTime createdAt,
            DateTime? revokedAt
        )
        {
            return new RefreshTokenDomainEntity
            {
                Id = id,
                AdminUserId = adminUserId,
                Token = token,
                JwtId = jwtId,
                IsRevoked = isRevoked,
                ExpiresAt = expiresAt,
                CreatedAt = createdAt,
                RevokedAt = revokedAt
            };
        }

        public void RevokeToken()
        {
            IsRevoked = true;
            RevokedAt = DateTime.UtcNow;
        }
    }
}