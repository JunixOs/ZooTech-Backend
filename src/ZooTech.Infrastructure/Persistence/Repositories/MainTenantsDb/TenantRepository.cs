using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Common.Gateway.Repositories.MainTenantsDb;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.ListTenants;
using ZooTech.Domain.Admin.Entities;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Mappers.MainTenantsDb;

namespace ZooTech.Infrastructure.Persistence.Repositories.MainTenantsDb
{
    public class TenantRepository : ITenantRepository
    {
        private readonly TenantCatalogDb _tenantDbContext;

        public TenantRepository(
            TenantCatalogDb tenantCatalogDb
        )
        {
            _tenantDbContext = tenantCatalogDb;
        }

        public async Task<TenantDomainEntity?> GetByIdAsync(int id)
        {
            var entity = await _tenantDbContext.tenants.FindAsync(id);
            return entity == null ? null : TenantMapper.ToDomain(entity);
        }

        public async Task<List<TenantDomainEntity>> ListAllAsync()
        {
            var entities = await _tenantDbContext.tenants.AsNoTracking().ToListAsync();
            return entities.Select(TenantMapper.ToDomain).ToList();
        }

        public async Task<List<ListTenantsOutput>> ListAllTenants()
        {
            return await _tenantDbContext.tenants
                .Select(t => new ListTenantsOutput
                {
                    Code = t.code,
                    SubDomain = t.subdomain,
                    LegalName = t.legal_name,
                    Email = t.email,
                    Phone = t.phone,
                    Status = t.status,
                    CreatedAt = t.created_at
                })
                .ToListAsync();
        }

    }
}
