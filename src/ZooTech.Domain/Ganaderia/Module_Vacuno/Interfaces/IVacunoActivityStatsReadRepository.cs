namespace ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;

public interface IVacunoActivityStatsReadRepository
{
    Task<IReadOnlyList<VacunoActivityStatsReadItem>> ListarHastaAsync(
        DateOnly fechaFin,
        CancellationToken cancellationToken = default);
}

public sealed record VacunoActivityStatsReadItem(
    DateOnly FechaRegistro,
    DateTime? DeletedAt);
