using ZooTech.Domain.Admin.Enums;

namespace ZooTech.Domain.Admin.Entities
{
    public class TenantDomainEntity
    {
        public int Id { get; private set; }
        public string Code { get; private set; } = default!;
        public string SubDomain { get; private set; } = default!;
        public string DisplayName { get; private set; } = default!;
        public string LegalName { get; private set; } = default!;
        public string Email { get; private set; } = default!;
        public string Phone { get; private set; } = default!;
        public TenantStatus Status { get; private set; }
        public string Metadata { get; private set; } = default!;
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private TenantDomainEntity() { }

        public static TenantDomainEntity Create(
            int id,
            string code,
            string subDomain,
            string displayName,
            string legalName,
            string email,
            string phone,
            TenantStatus status,
            DateTime? createdAt,
            DateTime updatedAt
        )
        {
            return new TenantDomainEntity
            {
                Id = id,
                Code = code,
                SubDomain = subDomain,
                DisplayName = displayName,
                LegalName = legalName,
                Email = email,
                Phone = phone,
                Status = status,
                CreatedAt = createdAt.GetValueOrDefault(DateTime.UtcNow),
                UpdatedAt = updatedAt
            };
        }

        public void UpdateContactInfo(string email, string phone)
        {
            Email = email;
            Phone = phone;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateStatus(TenantStatus status)
        {
            Status = status;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
