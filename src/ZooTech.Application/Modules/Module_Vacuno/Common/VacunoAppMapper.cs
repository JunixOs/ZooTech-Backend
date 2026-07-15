using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities;

namespace ZooTech.Application.Modules.Module_Vacuno.Common;

internal static class VacunoAppMapper
{
    internal static VacunoOutput ToOutput(Vacuno vacuno)
        => new(
            vacuno.Id,
            vacuno.Codigo,
            vacuno.Nombre,
            vacuno.FechaNacimiento,
            vacuno.TipoAdquisicionCode,
            vacuno.RazaCode,
            vacuno.ColorCode,
            vacuno.SexoCode,
            vacuno.PadreId,
            vacuno.MadreId,
            vacuno.GranjaId,
            vacuno.Observaciones,
            vacuno.FechaRegistro,
            vacuno.CreatedAt,
            vacuno.UpdatedAt);
}
