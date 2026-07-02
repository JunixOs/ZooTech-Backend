using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Common.Gateway.Repositories.MainTenantsDb;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.ListTenants;
using ZooTech.Domain.Admin.Entities;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Mappers.MainTenantsDb;
using ZooTech.Infrastructure.Tenant;

namespace ZooTech.Infrastructure.Persistence.Repositories.MainTenantsDb
{
    public class TenantRepository : ITenantRepository
    {
        private readonly TenantCatalogDb _context;

        public TenantRepository(ITenantDbContextFactory contextFactory)
        {
            _context = contextFactory.CreateDbContext();
        }

        public async Task<TenantDomainEntity?> GetByIdAsync(int id)
        {
            var entity = await _context.tenants.FindAsync(id);
            return entity == null ? null : TenantMapper.ToDomain(entity);
        }

        public async Task<List<TenantDomainEntity>> ListAllAsync()
        {
            var entities = await _context.tenants.AsNoTracking().ToListAsync();
            return entities.Select(TenantMapper.ToDomain).ToList();
        }

        public Task<List<ListTenantsOutput?>> ListAllTenants()
        {
            throw new NotImplementedException();
        }

    }
}
