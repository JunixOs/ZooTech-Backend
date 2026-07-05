using ZooTech.Domain.Module_Fecundacion.Entities;
using ZooTech.Domain.Module_Fecundacion.ReadModels;

namespace ZooTech.Domain.Module_Fecundacion.Interfaces;

public interface IFecundacionRepository
{
    Task<(List<FecundacionListItem> Items, int TotalCount)> GetPagedAsync(
    string? query, DateTime? fechaDesde, DateTime? fechaHasta, string? resultado,
    int page, int limit, CancellationToken cancellationToken = default);
    Task<Fecundacion> AddAsync(Fecundacion fecundacion, CancellationToken cancellationToken = default);

    Task<bool> ExistsVacunoAsync(long vacunoId, CancellationToken cancellationToken = default);

    Task<bool> ExistsCeloAsync(long celoId, CancellationToken cancellationToken = default);

    Task<bool> ExistsCodigoAsync(string codigo, CancellationToken cancellationToken = default);

    Task<long> GetOrCreateResponsableByNameAsync(string name, CancellationToken cancellationToken = default);
}
