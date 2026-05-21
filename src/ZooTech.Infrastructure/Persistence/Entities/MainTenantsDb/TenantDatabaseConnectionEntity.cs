namespace ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb
{
    public class TenantDatabaseConnectionEntity
    {
        public long Id { get; set; }
        public long TenantId {get; set;} = default!;
        public TenantEntity TenantEntity { get; set; } = default!;
        public required string DatabaseName { get; set; }
        public bool IsActive { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    } 
}