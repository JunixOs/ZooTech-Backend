using Microsoft.EntityFrameworkCore;
using ZooTech.Domain.Module_Sanidad.Interfaces;
using ZooTech.Infrastructure.Context;
using ZooTech.Infrastructure.Persistence.Modules.Module_Sanidad.Entities;

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
        return await _context.Triajes
            .AsNoTracking()
            .FirstOrDefaultAsync(triaje => triaje.Id == id && triaje.DeletedAt == null);
    }

    public async Task<IEnumerable<object>> GetAllAsync()
    {
        return await _context.Triajes
            .AsNoTracking()
            .Where(triaje => triaje.DeletedAt == null)
            .OrderByDescending(triaje => triaje.FechaHora)
            .Cast<object>()
            .ToListAsync();
    }

    public async Task AddAsync(object triaje)
    {
        if (triaje is not Triaje entity)
        {
            throw new ArgumentException("El objeto debe ser una entidad Triaje", nameof(triaje));
        }

        _context.Triajes.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(object triaje)
    {
        if (triaje is not Triaje entity)
        {
            throw new ArgumentException("El objeto debe ser una entidad Triaje", nameof(triaje));
        }

        _context.Triajes.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var triaje = await _context.Triajes
            .FirstOrDefaultAsync(item => item.Id == id && item.DeletedAt == null);

        if (triaje is null)
        {
            return;
        }

        var now = DateTime.UtcNow;
        triaje.DeletedAt = now;
        triaje.UpdatedAt = now;

        await _context.SaveChangesAsync();
    }
   public async Task<string> GenerateCodigoAsync()
    {
        var lastCodigo = await _context.Triajes
            .Where(t => t.Codigo.StartsWith("TRI"))
            .OrderByDescending(t => t.Codigo)
            .Select(t => t.Codigo)
            .FirstOrDefaultAsync();
        var nextNumber = 1;
        if (!string.IsNullOrEmpty(lastCodigo) && lastCodigo.Length > 3 && int.TryParse(lastCodigo[3..], out var lastNumber))
        {
            nextNumber = lastNumber + 1;
        }
        return $"TRI{nextNumber:D3}";
    }
}