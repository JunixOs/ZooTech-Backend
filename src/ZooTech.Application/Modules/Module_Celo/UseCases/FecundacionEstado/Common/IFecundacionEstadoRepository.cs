namespace ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.Common;

public interface IFecundacionEstadoRepository
{
    Task<FecundacionEstadoSnapshot?> GetByVacunoIdAsync(
        long vacunoId,
        CancellationToken cancellationToken = default);

    Task<FecundacionEstadoSnapshot?> GetByFecundacionIdAsync(
        long fecundacionId,
        CancellationToken cancellationToken = default);

    Task<string?> GetEstadoCodeByNameAsync(
        string estado,
        CancellationToken cancellationToken = default);

    Task<bool> HasOtherActiveFecundacionAsync(
        long vacunoId,
        long fecundacionId,
        CancellationToken cancellationToken = default);

    Task UpdateEstadoAsync(
        long fecundacionId,
        string estadoCode,
        long updatedBy,
        CancellationToken cancellationToken = default);
}
