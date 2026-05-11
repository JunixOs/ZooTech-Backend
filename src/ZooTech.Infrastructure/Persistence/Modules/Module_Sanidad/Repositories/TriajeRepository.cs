using ZooTech.Domain.Module_Sanidad.Interfaces;
using ZooTech.Infrastructure.Context;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_Sanidad.Repositories;

public class TriajeRepository : ITriajeRepository
{
    private readonly ZootechContext _context;

    public TriajeRepository(ZootechContext context)
    {
        _context = context;
    }

    public async Task<object?> GetByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<object>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task AddAsync(object triaje)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(object triaje)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(long id)
    {
        throw new NotImplementedException();
    }
}