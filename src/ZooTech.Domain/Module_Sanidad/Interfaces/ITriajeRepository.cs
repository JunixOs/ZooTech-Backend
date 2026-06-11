using ZooTech.Domain.Module_Sanidad.Entities;

namespace ZooTech.Domain.Module_Sanidad.Interfaces;

public interface ITriajeRepository
{
    Task<Triaje?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Triaje> Items, int Total)> GetAllAsync(
     int pagina,
     int tamano,
     string? fecha = null,
     string? codigo = null,
     string? nombre = null,
     string? tipoPeso = null,
     decimal? pesoKg = null,
     CancellationToken cancellationToken = default);
    Task<Triaje> AddAsync(Triaje triaje, CancellationToken cancellationToken = default);
    Task UpdateAsync(Triaje triaje, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
    Task<string> GenerateCodigoAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<TriajeHistorialItem>> GetHistorialByVacunoIdAsync(long vacunoId, CancellationToken cancellationToken = default);
}
