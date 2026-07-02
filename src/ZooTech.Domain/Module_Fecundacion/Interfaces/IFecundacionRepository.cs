using ZooTech.Domain.Module_Fecundacion.Entities;

namespace ZooTech.Domain.Module_Fecundacion.Interfaces;

public interface IFecundacionRepository
{
    Task<Fecundacion> AddAsync(Fecundacion fecundacion, CancellationToken cancellationToken = default);

    Task<bool> ExistsVacunoAsync(long vacunoId, CancellationToken cancellationToken = default);

    Task<bool> ExistsCeloAsync(long celoId, CancellationToken cancellationToken = default);

    Task<bool> ExistsCodigoAsync(string codigo, CancellationToken cancellationToken = default);

    Task<long> GetOrCreateResponsableByNameAsync(string name, CancellationToken cancellationToken = default);
}
