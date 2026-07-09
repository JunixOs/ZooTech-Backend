using Microsoft.EntityFrameworkCore;
using ZooTech.Domain.Module_Vacuno.Interfaces;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_Vacuno.Repositories;

public sealed class VacunoMutationUnitOfWork : IVacunoMutationUnitOfWork
{
    private readonly GanaderiaDbContext _context;

    public VacunoMutationUnitOfWork(IGanaderiaDbContextFactory ganaderiaDbContextFactory)
    {
        _context = ganaderiaDbContextFactory.CreateDbContextByTenantContext();
    }

    public async Task<T> ExecuteInTransactionAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        var result = await operation(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return result;
    }

    public async Task<long> EnsureGranjaAsync(
        string nombre,
        string codigoDistrito,
        CancellationToken cancellationToken = default)
    {
        var granjaExistente = await _context.granjas
            .Where(g => g.nombre == nombre && g.distrito_codigo == codigoDistrito)
            .Select(g => (long?)g.id)
            .FirstOrDefaultAsync(cancellationToken);

        if (granjaExistente.HasValue)
        {
            return granjaExistente.Value;
        }

        var now = DateTime.UtcNow;
        var granja = new granja
        {
            nombre = nombre,
            distrito_codigo = codigoDistrito,
            activo = true,
            created_at = now,
            updated_at = now
        };

        _context.granjas.Add(granja);
        await _context.SaveChangesAsync(cancellationToken);

        return granja.id;
    }
}
