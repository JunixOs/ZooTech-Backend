using ZooTech.Domain.Entities;
using ZooTech.Domain.Enums;
using ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

namespace ZooTech.Infrastructure.Persistence.Mappers.MainTenantsDb
{
    public class TenantMapper
    {
        public static TenantDomainEntity ToDomain(TenantEntity tenantEntity)
        {
            return TenantDomainEntity.Create(
                tenantEntity.Id,
                tenantEntity.Code,
                tenantEntity.SubDomain,
                tenantEntity.DisplayName,
                tenantEntity.LegalName,
                tenantEntity.Email,
                tenantEntity.Phone,
                tenantEntity.Status,
                tenantEntity.CreatedAt,
                tenantEntity.UpdatedAt
            );
        }

        public static TenantEntity ToEntity(TenantDomainEntity tenantDomainEntity)
        {
            return new TenantEntity
            {
                Code = tenantDomainEntity.Code,
                SubDomain = tenantDomainEntity.SubDomain,
                DisplayName = tenantDomainEntity.DisplayName,
                LegalName = tenantDomainEntity.LegalName,
                Email = tenantDomainEntity.Email,
                Phone = tenantDomainEntity.Phone,
                Status = tenantDomainEntity.Status.ToString(),
                CreatedAt = tenantDomainEntity.CreatedAt,
                UpdatedAt = tenantDomainEntity.UpdatedAt
            };
        }
    }
}