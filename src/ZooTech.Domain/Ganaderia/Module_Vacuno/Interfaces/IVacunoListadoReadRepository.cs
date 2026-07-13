namespace ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;

public interface IVacunoListadoReadRepository
{
    Task<IReadOnlyList<VacunoListadoReadItem>> ListarAsync(
        VacunoListadoReadQuery query,
        CancellationToken cancellationToken = default);
}

public sealed record VacunoListadoReadQuery(
    string? Q,
    string? Estado,
    DateOnly? FechaDesde,
    DateOnly? FechaHasta);

public sealed record VacunoListadoReadItem(
    long Id,
    string Codigo,
    string Nombre,
    DateOnly FechaNacimiento,
    DateOnly FechaRegistro,
    string RazaCode,
    string SexoCode,
    string? Procedencia,
    bool IsDeleted);
