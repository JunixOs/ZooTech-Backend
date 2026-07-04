using Microsoft.EntityFrameworkCore;
using ZooTech.Domain.Module_Vacuno.Entities;
using ZooTech.Domain.Module_Vacuno.Interfaces;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Modules.Module_Vacuno.Mappers;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_Vacuno.Repositories;

public sealed class RegistroVacunoReadRepository : IRegistroVacunoReadRepository
{
    private readonly GanaderiaDbContext _context;

    public RegistroVacunoReadRepository(GanaderiaDbContext context)
    {
        _context = context;
    }

    public async Task<Vacuno?> ObtenerRegistroAsync(long vacunoId, CancellationToken cancellationToken = default)
    {
        var entity = await _context.vacunos
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.id == vacunoId, cancellationToken);

        if (entity is null)
            return null;

        return VacunoPersistenceMapper.ToDomain(entity);
    }

    public const string CatalogoRaza = "raza";
    public const string CatalogoSexo = "sexo";
    public const string CatalogoColor = "color";
    public const string CatalogoTipoAdquisicion = "tipoadquisicion";

    public async Task<Dictionary<long, string>> ObtenerCodigosVacunoBatchAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default)
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0) return new Dictionary<long, string>();

        return await _context.vacunos
            .AsNoTracking()
            .Where(x => idList.Contains(x.id))
            .ToDictionaryAsync(x => x.id, x => x.codigo, cancellationToken);
    }

    public async Task<(string Nombre, string? Distrito, string? Provincia, string? Departamento)?> ObtenerDetallesGranjaAsync(long granjaId, CancellationToken cancellationToken = default)
    {
        var granja = await _context.granjas
            .AsNoTracking()
            .Include(g => g.distrito_codigoNavigation)
                .ThenInclude(d => d.provincia_codigoNavigation)
                    .ThenInclude(p => p.departamento_codigoNavigation)
            .Where(g => g.id == granjaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (granja is null) return null;

        return (
            granja.nombre,
            granja.distrito_codigoNavigation?.nombre,
            granja.distrito_codigoNavigation?.provincia_codigoNavigation?.nombre,
            granja.distrito_codigoNavigation?.provincia_codigoNavigation?.departamento_codigoNavigation?.nombre
        );
    }

    public async Task<(decimal? PrecioCompra, string? Proveedor, DateOnly? FechaAdquisicion)?> ObtenerDetallesAdquisicionAsync(long vacunoId, CancellationToken cancellationToken = default)
    {
        var adq = await _context.vacuno_adquisicions
            .AsNoTracking()
            .Where(a => a.vacuno_id == vacunoId)
            .FirstOrDefaultAsync(cancellationToken);

        if (adq is null) return null;

        return (adq.precio_compra, adq.proveedor, adq.fecha_adquisicion);
    }

    public async Task<(string? EstadoCode, string? EstadoNombre, DateOnly? FechaEstado, string? Motivo)?> ObtenerEstadoActualAsync(long vacunoId, CancellationToken cancellationToken = default)
    {
        var est = await _context.vacuno_estado_historials
            .AsNoTracking()
            .Include(x => x.estado_codeNavigation)
            .Where(x => x.vacuno_id == vacunoId)
            .OrderByDescending(x => x.fecha_estado)
            .ThenByDescending(x => x.id)
            .FirstOrDefaultAsync(cancellationToken);

        if (est is null) return null;

        return (est.estado_code, est.estado_codeNavigation?.nombre, est.fecha_estado, est.motivo);
    }

    public async Task<Dictionary<string, string>> ObtenerNombresCatalogoBatchAsync(string tipoCatalogo, IEnumerable<string> codigos, CancellationToken cancellationToken = default)
    {
        var codigoList = codigos.Where(c => !string.IsNullOrWhiteSpace(c)).Distinct().ToList();
        if (codigoList.Count == 0) return new Dictionary<string, string>();

        return tipoCatalogo.ToLowerInvariant() switch
        {
            CatalogoRaza => await _context.cat_razas.Where(x => codigoList.Contains(x.code)).ToDictionaryAsync(x => x.code, x => x.nombre, cancellationToken),
            CatalogoSexo => await _context.cat_sexos.Where(x => codigoList.Contains(x.code)).ToDictionaryAsync(x => x.code, x => x.nombre, cancellationToken),
            CatalogoColor => await _context.cat_colors.Where(x => codigoList.Contains(x.code)).ToDictionaryAsync(x => x.code, x => x.nombre, cancellationToken),
            CatalogoTipoAdquisicion => await _context.cat_tipo_adquisicions.Where(x => codigoList.Contains(x.code)).ToDictionaryAsync(x => x.code, x => x.nombre, cancellationToken),
            _ => new Dictionary<string, string>()
        };
    }

    public async Task<string?> ObtenerUtilizacionActualAsync(long vacunoId, CancellationToken cancellationToken = default)
    {
        var uti = await _context.vacuno_utilizacion_historials
            .AsNoTracking()
            .Include(x => x.tipo_utilizacion_codeNavigation)
            .Where(x => x.vacuno_id == vacunoId)
            .OrderByDescending(x => x.created_at)
            .ThenByDescending(x => x.id)
            .FirstOrDefaultAsync(cancellationToken);

        return uti?.tipo_utilizacion_codeNavigation?.nombre;
    }

    public async Task<(long? Id, string? NombreOriginal, string? NombreAlmacenado, string? RutaArchivo, string? Extension, long? TamanoBytes)?> ObtenerFotoPrincipalAsync(long vacunoId, CancellationToken cancellationToken = default)
    {
        var foto = await _context.vacuno_fotos
            .AsNoTracking()
            .Include(x => x.archivo)
            .Where(x => x.vacuno_id == vacunoId)
            .FirstOrDefaultAsync(cancellationToken);

        if (foto?.archivo is null) return null;
        var a = foto.archivo;
        return (a.id, a.nombre_original, a.nombre_almacenado, a.ruta_archivo, a.extension, a.tamano_bytes);
    }

    public async Task<string?> ObtenerNombreUsuarioAsync(long? usuarioId, CancellationToken cancellationToken = default)
    {
        if (usuarioId is null) return null;
        return await _context.usuarios
            .Where(u => u.id == usuarioId)
            .Select(u => u.nombre_completo)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
