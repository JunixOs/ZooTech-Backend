namespace ZooTech.Domain.Module_Reproduccion.Interfaces;

public interface IVacunoReproduccionRepository
{
    Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default);
    Task<bool> IsHembraAsync(long id, CancellationToken cancellationToken = default);
    Task<bool> IsVivoAsync(long id, CancellationToken cancellationToken = default);
}
