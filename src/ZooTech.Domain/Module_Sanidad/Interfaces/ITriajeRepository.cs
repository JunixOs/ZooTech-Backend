using ZooTech.Domain.Module_Sanidad.Entities;

namespace ZooTech.Domain.Module_Sanidad.Interfaces;

public interface ITriajeRepository
{
    Task<Triaje?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Triaje> Items, int Total)> GetAllAsync(
     int pagina,
     int tamano,
     string? fechaInicio = null,
     string? fechaFin = null,
     string? codigo = null,
     string? nombre = null,
     string? tipoPeso = null,
     decimal? pesoKg = null,
     CancellationToken cancellationToken = default);
    Task<Triaje> AddAsync(Triaje triaje, CancellationToken cancellationToken = default);
    Task UpdateAsync(Triaje triaje, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
    Task<string> GenerateCodigoAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<TriajeHistorialItem>> GetHistorialByVacunoIdAsync(long vacunoId, string? desde = null, string? hasta = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<TriajeHistorialItem>> GetHistorialGeneralAsync(string? desde = null, string? hasta = null, CancellationToken cancellationToken = default);

    // Validaciones de existencia (Rendimiento optimizado con AnyAsync)
    Task<bool> ExisteVacunoAsync(long vacunoId);
    Task<bool> ExisteTipoPesoAsync(string tipoPesoCode);

    // Tipo peso
    Task<IEnumerable<TipoPeso>> GetAllTipoPesosAsync();
    // Vacuno id codigo nombre
    Task<IEnumerable<VacunoOption>> GetAllVacunosAsync();
}
