using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Domain.Shared.Enums;
using ZooTech.Infrastructure.Exceptions;
using ZooTech.Infrastructure.Tenant;

namespace ZooTech.Infrastructure.Persistence.Context
{
    public class DbContextFactory : IDbContextFactory
    {
        private readonly ITenantContext _tenantContext;
        private readonly IGanaderiaDbContextFactory _ganaderiaDbContextFactory;
        private readonly ITenantDbContextFactory _tenantDbContextFactory;

        public DbContextFactory(
            ITenantContext tenantContext,
            IGanaderiaDbContextFactory ganaderiaDbContextFactory,
            ITenantDbContextFactory tenantDbContextFactory
        )
        {
            _tenantContext = tenantContext;

            _ganaderiaDbContextFactory = ganaderiaDbContextFactory;
            _tenantDbContextFactory = tenantDbContextFactory;
        }

        public async Task<DbContext> AutoGetDbContext()
        {
            if(_tenantContext.Type == TenantType.Tenant)
            {
                return await _ganaderiaDbContextFactory.CreateDbContext();
            } else if(_tenantContext.Type == TenantType.Admin) {
                return await _tenantDbContextFactory.CreateDbContext();
            } else
            {
                throw new InvalidTenantTypeException(_tenantContext.Type.ToString());
            }
        }

        public async Task<GanaderiaDbContext> GetGanaderiaDbContext()
        {
            if(_tenantContext.Type == TenantType.Tenant)
            {
                return await _ganaderiaDbContextFactory.CreateDbContext();
            } 
            else
            {
                throw new InvalidDbContextAccess();
            }
        }

        public async Task<TenantCatalogDb> GetTenantDbContext()
        {
            if(_tenantContext.Type == TenantType.Admin)
            {
                return await _tenantDbContextFactory.CreateDbContext();
            } 
            else
            {
                throw new InvalidDbContextAccess();
            }
        }
    }
}