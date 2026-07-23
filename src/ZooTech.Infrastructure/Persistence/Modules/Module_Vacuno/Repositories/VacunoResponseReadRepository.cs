using Microsoft.EntityFrameworkCore;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities;
using ZooTech.Infrastructure.Persistence.Context;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_Vacuno.Repositories;

public sealed class VacunoResponseReadRepository : IVacunoResponseReadRepository
{
    private readonly GanaderiaDbContext _context;

    public VacunoResponseReadRepository(IGanaderiaDbContextFactory ganaderiaDbContextFactory)
    {
        _context = ganaderiaDbContextFactory.CreateDbContextByTenantContext();
    }

    public async Task<Dictionary<long, string>> GetActiveCodesByIdsAsync(
        IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0)
        {
            return new Dictionary<long, string>();
        }

        return await _context.vacunos
            .AsNoTracking()
            .Where(v => idList.Contains(v.id) && v.deleted_at == null)
            .ToDictionaryAsync(v => v.id, v => v.codigo, cancellationToken);
    }

    public Task<VacunoGranjaDetails?> GetGranjaDetailsAsync(
        long granjaId,
        CancellationToken cancellationToken = default)
    {
        return _context.granjas
            .AsNoTracking()
            .Where(g => g.id == granjaId)
            .Select(g => new VacunoGranjaDetails(
                g.nombre,
                g.distrito_codigo,
                g.distrito_codigoNavigation != null ? g.distrito_codigoNavigation.nombre : null,
                g.distrito_codigoNavigation != null && g.distrito_codigoNavigation.provincia_codigoNavigation != null
                    ? g.distrito_codigoNavigation.provincia_codigoNavigation.nombre
                    : null,
                g.distrito_codigoNavigation != null
                    && g.distrito_codigoNavigation.provincia_codigoNavigation != null
                    && g.distrito_codigoNavigation.provincia_codigoNavigation.departamento_codigoNavigation != null
                        ? g.distrito_codigoNavigation.provincia_codigoNavigation.departamento_codigoNavigation.nombre
                        : null))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<decimal?> GetPrecioCompraAsync(
        long vacunoId,
        CancellationToken cancellationToken = default)
    {
        var adquisicion = await _context.vacuno_adquisicions
            .AsNoTracking()
            .Where(a => a.vacuno_id == vacunoId)
            .Select(a => new { a.precio_compra })
            .FirstOrDefaultAsync(cancellationToken);

        return adquisicion?.precio_compra;
    }

    public Task<VacunoUtilizacionDetails?> GetLatestUtilizacionAsync(
        long vacunoId,
        CancellationToken cancellationToken = default)
    {
        return _context.vacuno_utilizacion_historials
            .AsNoTracking()
            .Where(u => u.vacuno_id == vacunoId)

            .OrderByDescending(u => u.created_at)
            .Select(u => new VacunoUtilizacionDetails(
                u.tipo_utilizacion_code,
                u.created_at))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<string?> GetFotoUrlAsync(
        long vacunoId,
        CancellationToken cancellationToken = default)
    {
        return _context.vacuno_fotos
            .AsNoTracking()
            .Where(photo => photo.vacuno_id == vacunoId && photo.es_principal)
            .Select(photo => photo.archivo.ruta_archivo)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
