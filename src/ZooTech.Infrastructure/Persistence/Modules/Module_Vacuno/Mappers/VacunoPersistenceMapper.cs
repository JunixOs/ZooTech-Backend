using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities;
using ZooTech.Infrastructure.Persistence.Entities;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_Vacuno.Mappers;

internal static class VacunoPersistenceMapper
{
    public static vacuno ToEntity(Vacuno domain)
        => new()
        {
            codigo = domain.Codigo,
            nombre = domain.Nombre,
            fecha_nacimiento = domain.FechaNacimiento,
            tipo_adquisicion_code = domain.TipoAdquisicionCode,
            raza_code = domain.RazaCode,
            color_code = domain.ColorCode,
            sexo_code = domain.SexoCode,
            padre_id = domain.PadreId,
            madre_id = domain.MadreId,
            granja_id = domain.GranjaId,
            observaciones = domain.Observaciones,
            fecha_registro = domain.FechaRegistro,
            created_by = domain.CreatedBy,
            updated_by = domain.UpdatedBy,
            deleted_by = domain.DeletedBy,
            created_at = domain.CreatedAt,
            updated_at = domain.UpdatedAt,
            deleted_at = domain.DeletedAt,
            motivo_eliminacion = domain.MotivoEliminacion
        };

    public static Vacuno ToDomain(vacuno entity)
    {
        return Vacuno.Rehydrate(
            id: entity.id,
            codigo: entity.codigo,
            nombre: entity.nombre,
            fechaNacimiento: entity.fecha_nacimiento,
            tipoAdquisicionCode: entity.tipo_adquisicion_code,
            razaCode: entity.raza_code,
            colorCode: entity.color_code,
            sexoCode: entity.sexo_code,
            padreId: entity.padre_id,
            madreId: entity.madre_id,
            granjaId: entity.granja_id,
            observaciones: entity.observaciones,
            fechaRegistro: entity.fecha_registro,
            createdAt: entity.created_at,
            updatedAt: entity.updated_at,
            deletedAt: entity.deleted_at,
            motivoEliminacion: entity.motivo_eliminacion,
            createdBy: entity.created_by,
            updatedBy: entity.updated_by,
            deletedBy: entity.deleted_by);
    }
}
