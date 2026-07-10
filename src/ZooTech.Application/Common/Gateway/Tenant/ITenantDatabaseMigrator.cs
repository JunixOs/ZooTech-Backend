namespace ZooTech.Application.Common.Gateway.Tenant
{
    public interface ITenantDatabaseMigrator
    {
        Task MigrateAsync(string tenantDatabaseName);
    }
}