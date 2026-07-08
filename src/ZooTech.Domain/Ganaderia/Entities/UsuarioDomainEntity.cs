using ZooTech.Domain.Shared.ValueObjects;

namespace ZooTech.Domain.Ganaderia.Entities
{
    public class UsuarioDomainEntity
    {
        public int Id { get; private set; }
        public string? Code { get; private set; }
        public string? UserName { get; private set; }
        public string? FullName { get; private set; }
        public Email Email { get; private set; } = default!;
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public UsuarioDomainEntity() {}

        public static UsuarioDomainEntity Create(
            int id,
            string? code,
            string? username,
            string fullName,
            Email email,
            bool isActive,
            DateTime createdAt,
            DateTime? updatedAt
        )
        {
            return new UsuarioDomainEntity
            {
                Id = id,
                Code = code,
                UserName = username,
                FullName = fullName,
                Email = email,
                IsActive = isActive,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };
        }

        public static UsuarioDomainEntity CreateFromRequest(
            string? code,
            string? username,
            string fullName,
            Email email,
            bool isActive
        )
        {
            return new UsuarioDomainEntity
            {
                Code = code,
                UserName = username,
                FullName = fullName,
                Email = email,
                IsActive = isActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };
        }

        public void UpdateContactInfo(Email email)
        {
            Email = email;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateName(string username, string fullName)
        {
            UserName = username;
            FullName = fullName;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateStatus()
        {
            IsActive = !IsActive;
        }
    }
}