using Microsoft.EntityFrameworkCore;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;
using ZooTech.Infrastructure.Persistence.Context;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_Vacuno.Repositories;

public sealed class ListadoVacunosReporteReadRepository : IListadoVacunosReporteReadRepository
{
    private readonly GanaderiaDbContext _context;

    public ListadoVacunosReporteReadRepository(IGanaderiaDbContextFactory ganaderiaDbContextFactory)
    {
        _context = ganaderiaDbContextFactory.CreateDbContextByTenantContext();
    }

    public async Task<ListadoVacunosReporteReadResult> ListarAsync(
        ListadoVacunosReporteReadQuery query,
        CancellationToken cancellationToken = default)
    {
        var source = _context.vacunos
            .AsNoTracking()
            .Select(v => new ListadoVacunosReporteProjection
            {
                Id = v.id,
                Codigo = v.codigo,
                FechaNacimiento = v.fecha_nacimiento,
                FechaRegistro = v.fecha_registro,
                Nombre = v.nombre,
                TipoAdquisicionCode = v.tipo_adquisicion_code,
                TipoAdquisicionNombre = v.tipo_adquisicion_codeNavigation.nombre,
                RazaCode = v.raza_code,
                RazaNombre = v.raza_codeNavigation.nombre,
                ColorCode = v.color_code,
                ColorNombre = v.color_codeNavigation.nombre,
                SexoCode = v.sexo_code,
                SexoNombre = v.sexo_codeNavigation.nombre,
                Granja = v.granja.nombre,
                Distrito = v.granja.distrito_codigoNavigation.nombre,
                Provincia = v.granja.distrito_codigoNavigation.provincia_codigoNavigation.nombre,
                Departamento = v.granja.distrito_codigoNavigation.provincia_codigoNavigation.departamento_codigoNavigation.nombre,
                EstadoCode = v.vacuno_estado_historials
                    .OrderByDescending(h => h.fecha_estado)
                    .ThenByDescending(h => h.id)
                    .Select(h => h.estado_code)
                    .FirstOrDefault(),
                EstadoNombre = v.vacuno_estado_historials
                    .OrderByDescending(h => h.fecha_estado)
                    .ThenByDescending(h => h.id)
                    .Select(h => h.estado_codeNavigation.nombre)
                    .FirstOrDefault(),
                UtilizacionCode = v.vacuno_utilizacion_historials
                    .OrderByDescending(u => u.created_at)
                    .ThenByDescending(u => u.id)
                    .Select(u => u.tipo_utilizacion_code)
                    .FirstOrDefault(),
                UtilizacionNombre = v.vacuno_utilizacion_historials
                    .OrderByDescending(u => u.created_at)
                    .ThenByDescending(u => u.id)
                    .Select(u => u.tipo_utilizacion_codeNavigation.nombre)
                    .FirstOrDefault(),
                DeletedAt = v.deleted_at
            });

        if (query.FechaDesde.HasValue)
        {
            source = source.Where(v => v.FechaRegistro >= query.FechaDesde.Value);
        }

        if (query.FechaHasta.HasValue)
        {
            source = source.Where(v => v.FechaRegistro <= query.FechaHasta.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Q))
        {
            var term = query.Q.Trim();
            source = source.Where(v => v.Codigo.Contains(term) || v.Nombre.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(query.Codigo))
        {
            var codigo = query.Codigo.Trim();
            source = source.Where(v => v.Codigo.Contains(codigo));
        }

        if (query.FechaRegistro.HasValue)
        {
            source = source.Where(v => v.FechaRegistro == query.FechaRegistro.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Nombre))
        {
            var nombre = query.Nombre.Trim();
            source = source.Where(v => v.Nombre.Contains(nombre));
        }

        if (!string.IsNullOrWhiteSpace(query.Raza))
        {
            var raza = query.Raza.Trim();
            var razaCode = raza.ToUpperInvariant();
            source = source.Where(v =>
                v.RazaCode == razaCode ||
                (v.RazaNombre != null && v.RazaNombre == raza));
        }

        if (!string.IsNullOrWhiteSpace(query.Procedencia))
        {
            var procedencia = query.Procedencia.Trim();
            source = source.Where(v =>
                (v.Granja != null && v.Granja.Contains(procedencia)) ||
                (v.Distrito != null && v.Distrito.Contains(procedencia)) ||
                (v.Provincia != null && v.Provincia.Contains(procedencia)) ||
                (v.Departamento != null && v.Departamento.Contains(procedencia)));
        }

        if (!string.IsNullOrWhiteSpace(query.Estado))
        {
            var estado = query.Estado.Trim();
            var estadoLower = estado.ToLowerInvariant();

            if (estadoLower == "vivo")
            {
                source = source.Where(v => v.DeletedAt == null && v.EstadoCode != "MUERTO");
            }
            else if (estadoLower == "muerto")
            {
                source = source.Where(v => v.DeletedAt != null || v.EstadoCode == "MUERTO");
            }
            else
            {
                var estadoCode = estado.ToUpperInvariant();
                source = source.Where(v => v.EstadoCode == estadoCode);
            }
        }

        if (!string.IsNullOrWhiteSpace(query.EstadoRegistro))
        {
            source = query.EstadoRegistro == "eliminado"
                ? source.Where(v => v.DeletedAt != null)
                : source.Where(v => v.DeletedAt == null);
        }

        if (!string.IsNullOrWhiteSpace(query.AptoPara))
        {
            var aptoPara = query.AptoPara.Trim();
            var aptoParaCode = aptoPara.ToUpperInvariant();
            source = source.Where(v =>
                (v.UtilizacionCode != null && v.UtilizacionCode == aptoParaCode) ||
                (v.UtilizacionNombre != null && v.UtilizacionNombre == aptoPara));
        }

        var total = await source.CountAsync(cancellationToken);
        var skip = (query.Page - 1) * query.Limit;

        var rawItems = await source
            .OrderByDescending(v => v.FechaRegistro)
            .ThenBy(v => v.Codigo)
            .Skip(skip)
            .Take(query.Limit)
            .ToListAsync(cancellationToken);

        var items = rawItems
            .Select(v => new VacunoListadoReporteReadItem(
                v.Id,
                v.Codigo,
                v.FechaNacimiento,
                v.FechaRegistro,
                v.Nombre,
                v.TipoAdquisicionNombre ?? v.TipoAdquisicionCode,
                v.RazaNombre ?? v.RazaCode,
                v.ColorNombre ?? v.ColorCode,
                v.SexoNombre ?? v.SexoCode,
                v.Granja,
                BuildProcedencia(v.Granja, v.Distrito, v.Provincia, v.Departamento),
                v.DeletedAt != null || v.EstadoCode == "MUERTO" ? "muerto" : "vivo",
                v.DeletedAt is null ? "activo" : "eliminado"))
            .ToList();

        return new ListadoVacunosReporteReadResult(items, total);
    }

    private static string? BuildProcedencia(params string?[] values)
    {
        var parts = values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value!.Trim());

        return string.Join(", ", parts);
    }

    private sealed class ListadoVacunosReporteProjection
    {
        public long Id { get; init; }
        public string Codigo { get; init; } = string.Empty;
        public DateOnly FechaNacimiento { get; init; }
        public DateOnly FechaRegistro { get; init; }
        public string Nombre { get; init; } = string.Empty;
        public string TipoAdquisicionCode { get; init; } = string.Empty;
        public string? TipoAdquisicionNombre { get; init; }
        public string RazaCode { get; init; } = string.Empty;
        public string? RazaNombre { get; init; }
        public string ColorCode { get; init; } = string.Empty;
        public string? ColorNombre { get; init; }
        public string SexoCode { get; init; } = string.Empty;
        public string? SexoNombre { get; init; }
        public string? Granja { get; init; }
        public string? Distrito { get; init; }
        public string? Provincia { get; init; }
        public string? Departamento { get; init; }
        public string? EstadoCode { get; init; }
        public string? EstadoNombre { get; init; }
        public string? UtilizacionCode { get; init; }
        public string? UtilizacionNombre { get; init; }
        public DateTime? DeletedAt { get; init; }
    }
}
