using ZooTech.Domain.Module_Reproduccion.Entities;

namespace ZooTech.Domain.Module_Reproduccion.Interfaces;

public interface IFecundacionRepository
{
    Task<Fecundacion?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Fecundacion> AddAsync(Fecundacion fecundacion, CancellationToken cancellationToken = default);
    Task<Fecundacion> UpdateAsync(Fecundacion fecundacion, CancellationToken cancellationToken = default);
    Task<bool> HasPendingFecundacionAsync(long vacunoReceptorId, CancellationToken cancellationToken = default);
    Task<string> GenerateNextCodeAsync(CancellationToken cancellationToken = default);
    Task AssociateCriaAsync(long fecundacionId, long vacunoHijoId, CancellationToken cancellationToken = default);
}
