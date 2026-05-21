namespace ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb
{
    public class AddressEntity
    {
        public long Id { get; set; } = default!;
        public long TenantId { get; set; } = default!;
        public TenantEntity TenantEntity { get; set; } = default!;
        public required string Country { get; set; }
        public required string State { get; set; }
        public required string Province { get; set; }
        public required string City { get; set; }
        public required string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? Metadata { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime DeletedAt { get; set; }
    }
}