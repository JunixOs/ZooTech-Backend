namespace ZooTech.Domain.Module_Vacuno.Interfaces;

public interface IListadoVacunosReporteReadRepository
{
    Task<ListadoVacunosReporteReadResult> ListarAsync(
        ListadoVacunosReporteReadQuery query,
        CancellationToken cancellationToken = default);
}

public sealed record ListadoVacunosReporteReadQuery(
    DateOnly? FechaDesde,
    DateOnly? FechaHasta,
    string? Q,
    string? Codigo,
    DateOnly? FechaRegistro,
    string? Nombre,
    string? Raza,
    string? Procedencia,
    string? Estado,
    string? EstadoRegistro,
    string? AptoPara,
    int Page,
    int Limit);

public sealed record ListadoVacunosReporteReadResult(
    IReadOnlyCollection<VacunoListadoReporteReadItem> Items,
    int Total);

public sealed record VacunoListadoReporteReadItem(
    long Id,
    string Codigo,
    DateOnly FechaNacimiento,
    DateOnly FechaRegistro,
    string Nombre,
    string? TipoAdquisicion,
    string? Raza,
    string? Color,
    string? Sexo,
    string? Granja,
    string? Procedencia,
    string Estado,
    string EstadoRegistro);
