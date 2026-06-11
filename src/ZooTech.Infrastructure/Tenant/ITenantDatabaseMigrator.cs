namespace ZooTech.Infrastructure.Tenant
{
    public interface ITenantDatabaseMigrator
    {
        Task MigrateAsync(string connectionString);
    }
}