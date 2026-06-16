using ZooTech.Domain.Entities;
using ZooTech.Domain.Enums;
using ZooTech.Domain.ValueObjects;
using ZooTech.Infrastructure.Persistence.Entities;

namespace ZooTech.Infrastructure.Persistence.Mappers;

public static class VacunoMapper
{
    /// <summary>
    /// Convierte una entidad ORM vacuno (con navigations cargadas) a la entidad de dominio Animal.
    /// Requiere que las siguientes navigations estén incluidas:
    /// - raza_codeNavigation
    /// - granja → distrito_codigoNavigation → provincia_codigoNavigation → departamento_codigoNavigation
    /// </summary>
    /// <param name="entity">Entidad ORM vacuno con navigations cargadas.</param>
    /// <param name="estadoCode">Código del estado vigente desde v_vacuno_estado_vigente.</param>
    public static Animal ToDomain(vacuno entity, string? estadoCode)
    {
        // Mapear estado: si no hay historial, default VIVO
        var estado = EstadoAnimal.VIVO;
        if (!string.IsNullOrWhiteSpace(estadoCode))
        {
            Enum.TryParse<EstadoAnimal>(estadoCode, ignoreCase: true, out estado);
        }

        // Mapear raza desde catálogo
        var raza = new Raza(
            entity.raza_codeNavigation?.code ?? entity.raza_code,
            entity.raza_codeNavigation?.nombre ?? entity.raza_code);

        // Mapear procedencia desde la cadena granja → distrito → provincia → departamento
        var granjaNombre = entity.granja?.nombre ?? "Sin granja";
        var distritoNombre = entity.granja?.distrito_codigoNavigation?.nombre ?? "Sin distrito";
        var provinciaNombre = entity.granja?.distrito_codigoNavigation?.provincia_codigoNavigation?.nombre ?? "Sin provincia";
        var departamentoNombre = entity.granja?.distrito_codigoNavigation?.provincia_codigoNavigation?.departamento_codigoNavigation?.nombre ?? "Sin departamento";

        var procedencia = new Procedencia(granjaNombre, distritoNombre, provinciaNombre, departamentoNombre);

        return Animal.Reconstitute(
            AnimalId.Of(entity.id),
            entity.codigo,
            entity.nombre,
            entity.fecha_nacimiento.ToDateTime(TimeOnly.MinValue),
            entity.fecha_registro.ToDateTime(TimeOnly.MinValue),
            estado,
            raza,
            procedencia,
            entity.created_at);
    }

    /// <summary>
    /// Convierte una entidad de dominio Animal a la entidad ORM vacuno.
    /// Solo mapea los campos que el dominio controla. Los campos FK de catálogos
    /// (color_code, sexo_code, tipo_adquisicion_code, granja_id) deben ser
    /// establecidos por el caso de uso o el comando que crea el vacuno.
    /// </summary>
    public static vacuno ToEntity(Animal animal, string colorCode, string sexoCode, string tipoAdquisicionCode, long granjaId)
    {
        return new vacuno
        {
            id = animal.Id.Value,
            codigo = animal.Codigo,
            nombre = animal.Nombre,
            fecha_nacimiento = DateOnly.FromDateTime(animal.FechaNacimiento),
            fecha_registro = DateOnly.FromDateTime(animal.FechaRegistro),
            raza_code = animal.Raza.Code,
            color_code = colorCode,
            sexo_code = sexoCode,
            tipo_adquisicion_code = tipoAdquisicionCode,
            granja_id = granjaId,
            created_at = animal.CreatedAt,
            updated_at = animal.CreatedAt
        };
    }
}
