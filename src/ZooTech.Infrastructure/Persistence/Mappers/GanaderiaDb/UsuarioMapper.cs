using ZooTech.Domain.Ganaderia.Entities;
using ZooTech.Domain.Shared.ValueObjects;
using ZooTech.Infrastructure.Persistence.Entities;

namespace ZooTech.Infrastructure.Persistence.Mappers.GanaderiaDb
{
    public class UsuarioMapper
    {
        public static UsuarioDomainEntity toDomain(usuario orm)
        {
            return UsuarioDomainEntity.Create(
                (int)orm.id,
                orm.codigo,
                orm.nombre_usuario,
                orm.nombre_completo,
                new Email(orm.correo),
                orm.activo,
                orm.created_at,
                orm.updated_at
            );
        }

        public static usuario toOrm(UsuarioDomainEntity domain)
        {
            return new usuario
            {
                id = domain.Id,
                codigo = domain.Code,
                nombre_usuario = domain.UserName,
                nombre_completo = domain.FullName,
                correo = domain.Email.Value,
                activo = domain.IsActive,
                created_at = domain.CreatedAt,
                updated_at = domain.UpdatedAt.GetValueOrDefault()
            };
        }
    }
}