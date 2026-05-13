namespace ZooTech.Domain.Module_Sanidad.Interfaces;

public interface ITriajeRepository
{
    Task<object?> GetByIdAsync(long id);
    Task<IEnumerable<object>> GetAllAsync();
    Task AddAsync(object triaje);
    Task UpdateAsync(object triaje);
    Task DeleteAsync(long id);
    Task<string> GenerateCodigoAsync();
}