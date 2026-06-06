using ZooTech.Domain.Entities;
using ZooTech.Domain.Enums;
using ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

namespace ZooTech.Infrastructure.Persistence.Mappers.MainTenantsDb
{
    public class TenantMapper
    {
        public static TenantDomainEntity ToDomain(tenant tenantEntity)
        {
            return TenantDomainEntity.Create(
                tenantEntity.id,
                tenantEntity.code,
                tenantEntity.subdomain,
                tenantEntity.display_name,
                tenantEntity.legal_name,
                tenantEntity.email,
                tenantEntity.phone,
                tenantEntity.status,
                tenantEntity.created_at,
                tenantEntity.updated_at.GetValueOrDefault()
            );
        }

        public static tenant ToEntity(TenantDomainEntity tenantDomainEntity)
        {
            return new tenant
            {
                code = tenantDomainEntity.Code,
                subdomain = tenantDomainEntity.SubDomain,
                display_name = tenantDomainEntity.DisplayName,
                legal_name = tenantDomainEntity.LegalName,
                email = tenantDomainEntity.Email,
                phone = tenantDomainEntity.Phone,
                status = tenantDomainEntity.Status.ToString(),
                created_at = tenantDomainEntity.CreatedAt,
                updated_at = tenantDomainEntity.UpdatedAt
            };
        }
    }
}