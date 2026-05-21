namespace ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb
{
    public class TenantBrandingEntitiy
    {
        public long Id { get; set; }
        public long TenantId { get; set; }
        public TenantEntity Tenant { get; set; } = default!;
        public string? PrimaryColor { get; set; }
        public string? SecondaryColor { get; set; }
        public string? LogoUrl { get; set; }
        public string? Metadata { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}