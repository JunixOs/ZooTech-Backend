using ZooTech.Domain.Module_Sanidad.Entities;

namespace ZooTech.Domain.Module_Sanidad.Interfaces;

public interface ITriajeRepository
{
    Task<Triaje?> GetByIdAsync(long id);
    Task<(IEnumerable<Triaje> Items, int Total)> GetAllAsync(
     int pagina,
     int tamano,
     string? fecha = null,
     string? codigo = null,
     string? nombre = null,
     string? tipoPeso = null,
     decimal? pesoKg = null);
    Task AddAsync(Triaje triaje);
    Task UpdateAsync(Triaje triaje);
    Task DeleteAsync(long id);
    Task<string> GenerateCodigoAsync();

    // Validaciones de existencia (Rendimiento optimizado con AnyAsync)
    Task<bool> ExisteVacunoAsync(long vacunoId);
    Task<bool> ExisteTipoPesoAsync(string tipoPesoCode);

    // Tipo peso
    Task<IEnumerable<TipoPeso>> GetAllTipoPesosAsync();
    // Vacuno id codigo nombre
    Task<IEnumerable<VacunoOption>> GetAllVacunosAsync();

    Task<IEnumerable<TriajeHistorialItem>> GetHistorialByVacunoIdAsync(long vacunoId);
}