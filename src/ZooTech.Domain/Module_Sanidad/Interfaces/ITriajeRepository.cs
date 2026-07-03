using ZooTech.Domain.Module_Sanidad.Entities;

namespace ZooTech.Domain.Module_Sanidad.Interfaces;

public interface ITriajeRepository
{
    Task<Triaje?> GetByIdAsync(long id);
    Task<(IEnumerable<Triaje> Items, int Total)> GetAllAsync(
        int pagina,
        int tamano,
        string? desde = null,
        string? hasta = null,
        string? codigo = null,
        string? nombre = null,
        string? tipoPeso = null,
        decimal? pesoKg = null);
    Task AddAsync(Triaje triaje);
    Task UpdateAsync(Triaje triaje);
    Task DeleteAsync(long id);
    Task<string> GenerateCodigoAsync();

    // Tipo peso
    Task<IEnumerable<TipoPeso>> GetAllTipoPesosAsync();
    // Vacuno id codigo nombre
    Task<IEnumerable<VacunoOption>> GetAllVacunosAsync();

    Task<IEnumerable<TriajeHistorialItem>> GetHistorialByVacunoIdAsync(long vacunoId, string? desde = null, string? hasta = null);
    Task<IEnumerable<TriajeHistorialItem>> GetHistorialGeneralAsync(string? desde = null, string? hasta = null);
    Task<IEnumerable<TriajeDetallePorVacunoItem>> GetDetallesByVacunoIdAsync(long vacunoId);
}