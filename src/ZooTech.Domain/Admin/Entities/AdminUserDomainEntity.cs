using ZooTech.Domain.Shared.ValueObjects;

namespace ZooTech.Domain.Admin.Entities
{
    public class AdminUserDomainEntity
    {
        public int Id { get; private set; }
        public Email Email { get; private set; } = default!;
        public string? UserName { get; private set; }
        public string PasswordHash { get; private set; } = default!;
        public string? FirstName { get; private set; }
        public string? LastName { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime? LastLoginAt { get; private set; }
        public string? Metadata { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? DeletedAt { get; private set; }

        private AdminUserDomainEntity() { }

        public static AdminUserDomainEntity Create(
            int id,
            Email email,
            string? userName,
            string passwordHash,
            string? firstName,
            string? lastName,
            bool isActive,
            DateTime? lastLoginAt,
            string? metadata,
            DateTime? updatedAt,
            DateTime? createdAt,
            DateTime? deletedAt
        )
        {
            return new AdminUserDomainEntity
            {
                Id = id,
                Email = email,
                UserName = userName,
                PasswordHash = passwordHash,
                FirstName = firstName,
                LastName = lastName,
                IsActive = isActive,
                LastLoginAt = lastLoginAt,
                Metadata = metadata,
                UpdatedAt = updatedAt,
                CreatedAt = createdAt.GetValueOrDefault(DateTime.UtcNow),
                DeletedAt = deletedAt
            };
        }

        public static AdminUserDomainEntity CreateFromCreationRequest(
            Email email,
            string? userName,
            string passwordHash,
            string? firstName,
            string? lastName,
            bool isActive,
            DateTime? lastLoginAt,
            string? metadata,
            DateTime? updatedAt,
            DateTime? createdAt,
            DateTime? deletedAt
        )
        {
            return new AdminUserDomainEntity
            {
                Email = email,
                UserName = userName,
                PasswordHash = passwordHash,
                FirstName = firstName,
                LastName = lastName,
                IsActive = isActive,
                LastLoginAt = lastLoginAt,
                Metadata = metadata,
                UpdatedAt = updatedAt,
                CreatedAt = createdAt.GetValueOrDefault(DateTime.UtcNow),
                DeletedAt = deletedAt
            };
        }

        public void UpdateContactInfo(Email email)
        {
            Email = email;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateName(string username, string firstName, string lastName)
        {
            UserName = username;
            FirstName = firstName;
            LastName = lastName;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdatePassword(string passwordHash)
        {
            PasswordHash = passwordHash;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateStatus(bool isActive)
        {
            IsActive = isActive;

            if (!isActive)
            {
                DeletedAt = DateTime.UtcNow;
            }
            else
            {
                DeletedAt = null;
            }
        }

        public void UpdateLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
        }
    }
}