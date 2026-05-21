namespace ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb
{
    // Entidad ORM
    public class TenantEntity
    {
        public long Id { get; set; }
        public required string Code { get; set; }
        public required string SubDomain { get; set; }
        public required string DisplayName { get; set; }
        public required string LegalName { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public AddressEntity Address { get; set; } = default!;
        public TenantDatabaseConnectionEntity TenantDatabaseConnection { get; set; } = default!;
        public TenantBrandingEntitiy TenantBranding { get; set; } = default!;
        public required string Status { get; set; } = "TRIAL";
        public string? Metadata {get; set;}
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
} 