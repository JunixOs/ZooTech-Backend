using Microsoft.EntityFrameworkCore;
using ZooTech.Domain.Entities;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Mappers.MainTenantsDb;

namespace ZooTech.Infrastructure.Persistence.Repositories.MainTenantsDb
{
    public class TenantRepository : Application.Common.Gateway.Repositories.MainTenantsDb.ITenantRepository
    {
        private readonly TenantCatalogDb _context;

        public TenantRepository(TenantCatalogDb context)
        {
            _context = context;
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
    }
}
