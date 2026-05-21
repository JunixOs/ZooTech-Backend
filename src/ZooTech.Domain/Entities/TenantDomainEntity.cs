using ZooTech.Domain.Enums;

namespace ZooTech.Domain.Entities
{
    public class TenantDomainEntity
    {
        public long Id { get; set; }
        public string Code { get; private set; }
        public string SubDomain { get; set; }
        public string DisplayName { get; set; }
        public string LegalName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public TenantStatus Status { get; set; }
        public string Metadata {get; set;}
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; set; }
    
        private TenantDomainEntity() { }

        public static TenantDomainEntity Create(
            long id,
            string code,
            string subDomain,
            string displayName,
            string legalName,
            string email,
            string phone,
            string status,
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
                Status = Enum.Parse<TenantStatus>(status),
                CreatedAt = createdAt.GetValueOrDefault(DateTime.Now),
                UpdatedAt = updatedAt
            };
        }
    }
}