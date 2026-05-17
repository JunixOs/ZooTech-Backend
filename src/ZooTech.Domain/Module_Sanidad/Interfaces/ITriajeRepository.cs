using ZooTech.Domain.Module_Sanidad.Entities;

namespace ZooTech.Domain.Module_Sanidad.Interfaces;

public interface ITriajeRepository
{
    Task<Triaje?> GetByIdAsync(long id);
    Task<IEnumerable<Triaje>> GetAllAsync();
    Task AddAsync(Triaje triaje);
    Task UpdateAsync(Triaje triaje);
    Task DeleteAsync(long id);
    Task<string> GenerateCodigoAsync();

    // Tipo peso
    Task<IEnumerable<TipoPeso>> GetAllTipoPesosAsync();
    // Vacuno id codigo nombre
    Task<IEnumerable<VacunoOption>> GetAllVacunosAsync();

    Task<IEnumerable<TriajeHistorialItem>> GetHistorialByVacunoIdAsync(long vacunoId);
}