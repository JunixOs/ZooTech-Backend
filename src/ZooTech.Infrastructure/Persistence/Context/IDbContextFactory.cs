using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.Context
{
    public interface IDbContextFactory
    {
        Task<DbContext> AutoGetDbContext();
        Task<GanaderiaDbContext> GetGanaderiaDbContext();
        Task<TenantCatalogDb> GetTenantDbContext();
    }
}