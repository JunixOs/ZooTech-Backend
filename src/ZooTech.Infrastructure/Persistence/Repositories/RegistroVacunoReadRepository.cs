using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.ReadModels;

namespace ZooTech.Infrastructure.Persistence.Repositories;

public sealed class RegistroVacunoReadRepository : IRegistroVacunoReadRepository
{
    private readonly GanaderiaDbContext _context;

    public RegistroVacunoReadRepository(GanaderiaDbContext context)
    {
        _context = context;
    }

    public async Task<RegistroVacunoDetalle?> ObtenerRegistroAsync(
        long vacunoId,
        CancellationToken cancellationToken = default)
    {
        var v = await _context.vacunos
            .AsNoTracking()
            .Include(x => x.sexo_codeNavigation)
            .Include(x => x.raza_codeNavigation)
            .Include(x => x.color_codeNavigation)
            .Include(x => x.tipo_adquisicion_codeNavigation)
            .Include(x => x.vacuno_adquisicion)
            .Include(x => x.padre)
                .ThenInclude(p => p.padre)
            .Include(x => x.madre)
                .ThenInclude(m => m.madre)
            .Include(x => x.granja)
                .ThenInclude(g => g.distrito_codigoNavigation)
                    .ThenInclude(d => d.provincia_codigoNavigation)
                        .ThenInclude(p => p.departamento_codigoNavigation)
            .Include(x => x.vacuno_foto)
                .ThenInclude(vf => vf.archivo)
            .Include(x => x.vacuno_estado_historials)
                .ThenInclude(veh => veh.estado_codeNavigation)
            .Include(x => x.vacuno_utilizacion_historials)
                .ThenInclude(vuh => vuh.tipo_utilizacion_codeNavigation)
            .Include(x => x.created_byNavigation)
            .Include(x => x.updated_byNavigation)
            .FirstOrDefaultAsync(x => x.id == vacunoId && x.deleted_at == null, cancellationToken);

        if (v is null)
        {
            return null;
        }

        var veh = v.vacuno_estado_historials.OrderByDescending(h => h.id).FirstOrDefault();
        var vuh = v.vacuno_utilizacion_historials.OrderByDescending(h => h.created_at).ThenByDescending(h => h.id).FirstOrDefault();
        var vf = v.vacuno_foto;
        var a = vf?.archivo;

        return new RegistroVacunoDetalle(
            v.id,
            v.codigo,
            v.nombre,
            v.fecha_nacimiento,
            NormalizeCatalogValue(v.tipo_adquisicion_codeNavigation?.nombre),
            v.vacuno_adquisicion?.precio_compra,
            v.raza_codeNavigation?.nombre,
            v.color_codeNavigation?.nombre,
            NormalizeCatalogValue(v.sexo_codeNavigation?.nombre),
            v.padre?.codigo,
            v.madre?.codigo,
            v.padre?.padre?.codigo,
            v.madre?.madre?.codigo,
            v.granja?.nombre,
            v.granja?.distrito_codigoNavigation?.nombre,
            v.granja?.distrito_codigoNavigation?.provincia_codigoNavigation?.nombre,
            v.granja?.distrito_codigoNavigation?.provincia_codigoNavigation?.departamento_codigoNavigation?.nombre,
            v.vacuno_adquisicion?.proveedor,
            NormalizeCatalogValue(vuh?.tipo_utilizacion_codeNavigation?.nombre),
            v.vacuno_adquisicion?.fecha_adquisicion,
            v.observaciones,
            a?.id,
            a?.nombre_original,
            a?.nombre_almacenado,
            a?.ruta_archivo,
            a?.ruta_archivo,
            a?.extension,
            a?.tamano_bytes,
            veh?.estado_code,
            veh?.estado_codeNavigation?.nombre,
            DeterminarEstado(veh?.estado_code, veh?.estado_codeNavigation?.nombre),
            veh?.fecha_estado,
            veh?.motivo,
            v.fecha_registro,
            v.vacuno_adquisicion?.fecha_adquisicion,
            v.created_byNavigation?.nombre_completo,
            v.created_at,
            v.updated_byNavigation?.nombre_completo,
            v.updated_at);
    }

    private static string? NormalizeCatalogValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim().ToLowerInvariant().Replace(' ', '_');
    }

    private static string? DeterminarEstado(string? estadoCode, string? estadoNombre)
    {
        if (string.IsNullOrWhiteSpace(estadoCode))
            return NormalizeCatalogValue(estadoNombre);

        return estadoCode.ToUpperInvariant() switch
        {
            "ACTIVO" or "VIVO" => "vivo",
            "MUERTO" or "FALLECIDO" or "BAJA" => "muerto",
            _ => NormalizeCatalogValue(estadoNombre)
        };
    }
}

