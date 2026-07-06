using ZooTech.Domain.Module_ProduccionLeche.Entities;

namespace ZooTech.Domain.Module_ProduccionLeche.Interfaces;

public interface IOrdenioRepository
{
    Task<bool> ExistsCodigoAsync(string codigo, CancellationToken cancellationToken);
    Task<bool> ExistsVacunoFechaAsync(long vacunoId, DateTime fechaHora, long? excludeOrdenioId, CancellationToken cancellationToken);
    Task<bool> ExistsVacunoAsync(long vacunoId, CancellationToken cancellationToken);
    Task<bool> ExistsUsuarioAsync(long usuarioId, CancellationToken cancellationToken);
    Task<bool> ExistsEstadoAsync(string estadoOrdenioCode, CancellationToken cancellationToken);
    Task<Ordenio?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<(IReadOnlyList<OrdenioList> Items, int TotalCount)> ListAsync(
        long? vacunoId,
        string? estadoOrdenioCode,
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        int page,
        int pageSize,
        CancellationToken cancellationToken);
    Task<IReadOnlyList<OrdenioList>> ListReportAsync(
        long? vacunoId,
        string? estadoOrdenioCode,
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        CancellationToken cancellationToken);
    Task<Ordenio> AddAsync(Ordenio ordenio, CancellationToken cancellationToken);
    Task<Ordenio> UpdateAsync(Ordenio ordenio, CancellationToken cancellationToken);
}
