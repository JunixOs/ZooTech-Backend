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
        private readonly IDbContextFactory _dbContextFactory;

        public TenantRepository(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<TenantDomainEntity?> GetByIdAsync(int id)
        {
            var tenantDbContext = await _dbContextFactory.GetTenantDbContext();

            var entity = await tenantDbContext.tenants.FindAsync(id);
            return entity == null ? null : TenantMapper.ToDomain(entity);
        }

        public async Task<List<TenantDomainEntity>> ListAllAsync()
        {
            var tenantDbContext = await _dbContextFactory.GetTenantDbContext();

            var entities = await tenantDbContext.tenants.AsNoTracking().ToListAsync();
            return entities.Select(TenantMapper.ToDomain).ToList();
        }

        public async Task<List<ListTenantsOutput>> ListAllTenants()
        {
            var tenantDbContext = await _dbContextFactory.GetTenantDbContext();

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
