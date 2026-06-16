namespace ZooTech.Domain.Module_ProduccionLeche.Interfaces;

public sealed record VacasSequiaResult(
    IReadOnlyList<VacaSequiaActual> Actuales,
    IReadOnlyList<VacaSequiaFuturo> Futuros,
    IReadOnlyList<VacaSequiaHistorico> Historicos);

public interface IPeriodoSequiaRepository
{
    Task<VacasSequiaResult> GetVacasSequiaAsync(CancellationToken cancellationToken);
}
