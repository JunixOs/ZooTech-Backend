using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.ListOrdenios;
using ZooTech.Domain.Module_ProduccionLeche.Entities;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Ports;

public interface IOrdenioRepository
{
    Task<bool> ExistsCodigoAsync(string codigo, CancellationToken cancellationToken);
    Task<bool> ExistsVacunoFechaAsync(long vacunoId, DateTime fechaHora, long? excludeOrdenioId, CancellationToken cancellationToken);
    Task<bool> ExistsVacunoAsync(long vacunoId, CancellationToken cancellationToken);
    Task<bool> ExistsUsuarioAsync(long usuarioId, CancellationToken cancellationToken);
    Task<bool> ExistsEstadoAsync(string estadoOrdenioCode, CancellationToken cancellationToken);
    Task<Ordenio?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<(IReadOnlyList<OrdenioOutput> Items, int TotalCount)> ListAsync(ListOrdeniosQuery query, CancellationToken cancellationToken);
    Task<Ordenio> AddAsync(Ordenio ordenio, CancellationToken cancellationToken);
    Task<Ordenio> UpdateAsync(Ordenio ordenio, CancellationToken cancellationToken);
}
