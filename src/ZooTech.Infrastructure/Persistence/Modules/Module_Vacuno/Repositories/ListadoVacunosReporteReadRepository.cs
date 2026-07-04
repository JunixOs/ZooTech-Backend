using Microsoft.EntityFrameworkCore;
using ZooTech.Domain.Module_Vacuno.Interfaces;
using ZooTech.Infrastructure.Persistence.Context;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_Vacuno.Repositories;

public sealed class ListadoVacunosReporteReadRepository : IListadoVacunosReporteReadRepository
{
    private readonly GanaderiaDbContext _context;

    public ListadoVacunosReporteReadRepository(GanaderiaDbContext context)
    {
        _context = context;
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
                FechaRegistro = v.fecha_registro,
                Nombre = v.nombre,
                RazaCode = v.raza_code,
                RazaNombre = v.raza_codeNavigation.nombre,
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
            var estado = query.Estado.Trim().ToUpperInvariant();
            source = source.Where(v => v.EstadoCode == estado);
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
                v.FechaRegistro,
                v.Nombre,
                v.RazaNombre ?? v.RazaCode,
                BuildProcedencia(v.Granja, v.Distrito, v.Provincia, v.Departamento),
                v.EstadoCode ?? "SANO",
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
        public DateOnly FechaRegistro { get; init; }
        public string Nombre { get; init; } = string.Empty;
        public string RazaCode { get; init; } = string.Empty;
        public string? RazaNombre { get; init; }
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
