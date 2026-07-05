using ZooTech.Domain.Module_Vacuno.ReadModels.ListarVacuno;
using ZooTech.Infrastructure.Persistence.Models;

namespace ZooTech.Infrastructure.Persistence.Mappers;

public static class VacunoMapper
{
    public static VacunoListItem ToListItem(this VacunoRawRow r)
    {
        var procedencia = string.Join(", ",
            new[] { r.GranjaNombre, r.DistritoNombre, r.ProvinciaNombre, r.DepartamentoNombre }
            .Where(s => !string.IsNullOrWhiteSpace(s)));

        return new VacunoListItem(
            Id: r.Id,
            Codigo: r.Codigo,
            Nombre: r.Nombre,
            FechaNacimiento: r.FechaNacimiento,
            RazaCode: r.RazaCode ?? string.Empty,
            Procedencia: string.IsNullOrWhiteSpace(procedencia) ? null : procedencia,
            IsDeleted: r.DeletedAt != null,
            FechaRegistro: r.FechaRegistro
        );
    }
}