namespace ZooTech.Application.Common.Gateway.Tenant
{
    public interface ITenantDatabaseCreator
    {
        Task<bool> ExistsAsync(string databaseName);

        Task CreateAsync(string databaseName);

        Task DeleteAsync(string databaseName);
    }
}