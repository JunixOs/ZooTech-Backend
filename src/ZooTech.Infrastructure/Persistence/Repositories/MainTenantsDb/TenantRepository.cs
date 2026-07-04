using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Common.Gateway.Repositories.MainTenantsDb;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.ListTenants;
using ZooTech.Domain.Admin.Entities;
using ZooTech.Infrastructure.Persistence.Mappers.MainTenantsDb;
using ZooTech.Infrastructure.Tenant;

namespace ZooTech.Infrastructure.Persistence.Repositories.MainTenantsDb
{
    public class TenantRepository : ITenantRepository
    {
        private readonly ITenantDbContextFactory _tenantDbContextFactory;

        public TenantRepository(
            ITenantDbContextFactory tenantDbContextFactory
        )
        {
            _tenantDbContextFactory = tenantDbContextFactory;
        }

        public async Task<TenantDomainEntity?> GetByIdAsync(int id)
        {
            var tenantDbContext = await _tenantDbContextFactory.CreateDbContextByTenantContext();

            var entity = await tenantDbContext.tenants.FindAsync(id);
            return entity == null ? null : TenantMapper.ToDomain(entity);
        }

        public async Task<List<TenantDomainEntity>> ListAllAsync()
        {
            var tenantDbContext = await _tenantDbContextFactory.CreateDbContextByTenantContext();

            var entities = await tenantDbContext.tenants.AsNoTracking().ToListAsync();
            return entities.Select(TenantMapper.ToDomain).ToList();
        }

        public async Task<List<ListTenantsOutput>> ListAllTenants()
        {
            var tenantDbContext = await _tenantDbContextFactory.CreateDbContextByTenantContext();

            return await tenantDbContext.tenants
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
