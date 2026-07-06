using ZooTech.Domain.Admin.Entities;
using ZooTech.Domain.Shared.ValueObjects;
using ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

namespace ZooTech.Infrastructure.Persistence.Mappers.MainTenantsDb
{
    public class AdminUserMapper
    {
        public static AdminUserDomainEntity ToDomain(admin_user ormEntity)
        {
            return AdminUserDomainEntity.Create(
                ormEntity.id,
                new Email(ormEntity.email),
                ormEntity.username,
                ormEntity.password_hash,
                ormEntity.first_name,
                ormEntity.last_name,
                ormEntity.is_active,
                ormEntity.last_login_at,
                ormEntity.metadata,
                ormEntity.updated_at,
                ormEntity.created_at,
                ormEntity.deleted_at
            );
        }

        public static admin_user ToOrm(AdminUserDomainEntity domain)
        {
            return new admin_user
            {
                id = domain.Id,
                email = domain.Email.Value,
                username = domain.UserName,
                password_hash = domain.PasswordHash,
                first_name = domain.FirstName,
                last_name = domain.LastName,
                is_active = domain.IsActive,
                last_login_at = domain.LastLoginAt,
                metadata = domain.Metadata,
                created_at = domain.CreatedAt,
                updated_at = domain.UpdatedAt,
                deleted_at = domain.DeletedAt
            };
        }
    }
}