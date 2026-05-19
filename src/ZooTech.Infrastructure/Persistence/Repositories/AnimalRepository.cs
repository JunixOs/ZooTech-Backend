using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Common.Gateway.Repositories;
using ZooTech.Domain.Entities;
using ZooTech.Domain.Enums;
using ZooTech.Infrastructure.Persistence.Mappers;

namespace ZooTech.Infrastructure.Persistence.Repositories;

public class AnimalRepository : IAnimalRepository
{
    private readonly ITenantDbContextFactory _factory;

    public AnimalRepository(ITenantDbContextFactory factory)
    {
        _factory = factory;
    }

    public async Task<(List<Animal> Data, int Total)> GetPagedAsync(
        DateTime? fechaDesde, 
        DateTime? fechaHasta, 
        EstadoAnimal? estado, 
        string? q, 
        int skip, 
        int take)
    {
        using var db = _factory.CreateDbContext();

        var query = db.Animals.AsNoTracking().AsQueryable();

        if (fechaDesde.HasValue)
            query = query.Where(a => a.FechaRegistro >= fechaDesde.Value);

        if (fechaHasta.HasValue)
            query = query.Where(a => a.FechaRegistro <= fechaHasta.Value);

        if (estado.HasValue)
        {
            var estadoStr = estado.Value.ToString();
            query = query.Where(a => a.Estado == estadoStr);
        }

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(a => a.Codigo.Contains(q) || a.Nombre.Contains(q));
        }

        var total = await query.CountAsync();

        var entities = await query
            .OrderByDescending(a => a.FechaRegistro)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        var domainEntities = entities.Select(AnimalMapper.ToDomain).ToList();

        return (domainEntities, total);
    }
}
