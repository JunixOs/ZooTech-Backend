using Microsoft.EntityFrameworkCore;
using ZooTech.Domain.Module_Vacuno.Interfaces;
using ZooTech.Infrastructure.Persistence.Context;
using Entities = ZooTech.Infrastructure.Persistence.Entities;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_Vacuno.Repositories;

public sealed class ListadoVacunosReporteReadRepository : IListadoVacunosReporteReadRepository
{
    private static readonly string[] DeadStateCodes = ["MUERTO", "FALLECIDO", "BAJA", "INACTIVO"];
    private readonly GanaderiaDbContext _context;

    public ListadoVacunosReporteReadRepository(GanaderiaDbContext context)
    {
        _context = context;
    }

    public async Task<ListadoVacunosReporteReadResult> ListarAsync(
        ListadoVacunosReporteReadQuery query,
        CancellationToken cancellationToken = default)
    {
        var filteredVacunos = ApplyFilters(_context.vacunos.AsNoTracking(), query);
        var total = await filteredVacunos.CountAsync(cancellationToken);
        var rawItems = await LoadPageAsync(filteredVacunos, query, cancellationToken);

        var items = rawItems
            .Select(v => new VacunoListadoReporteReadItem(
                v.Id,
                v.Codigo,
                v.FechaRegistro,
                v.Nombre,
                v.RazaNombre ?? v.RazaCode,
                BuildProcedencia(v.Granja, v.Distrito, v.Provincia, v.Departamento),
                ToEstadoBiologico(v.EstadoCode),
                v.DeletedAt is null ? "activo" : "eliminado"))
            .ToList();

        return new ListadoVacunosReporteReadResult(items, total);
    }

    private IQueryable<Entities.vacuno> ApplyFilters(
        IQueryable<Entities.vacuno> source,
        ListadoVacunosReporteReadQuery query)
    {
        source = ApplyVacunoFilters(source, query);
        source = ApplyEstadoFilter(source, query.Estado);
        source = ApplyEstadoRegistroFilter(source, query.EstadoRegistro);
        source = ApplyAptoParaFilter(source, query.AptoPara);

        return source;
    }

    private static IQueryable<Entities.vacuno> ApplyVacunoFilters(
        IQueryable<Entities.vacuno> source,
        ListadoVacunosReporteReadQuery query)
    {
        if (query.FechaDesde.HasValue)
        {
            source = source.Where(v => v.fecha_registro >= query.FechaDesde.Value);
        }

        if (query.FechaHasta.HasValue)
        {
            source = source.Where(v => v.fecha_registro <= query.FechaHasta.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Q))
        {
            var term = query.Q.Trim();
            source = source.Where(v => v.codigo.Contains(term) || v.nombre.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(query.Codigo))
        {
            var codigo = query.Codigo.Trim();
            source = source.Where(v => v.codigo.Contains(codigo));
        }

        if (query.FechaRegistro.HasValue)
        {
            source = source.Where(v => v.fecha_registro == query.FechaRegistro.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Nombre))
        {
            var nombre = query.Nombre.Trim();
            source = source.Where(v => v.nombre.Contains(nombre));
        }

        if (!string.IsNullOrWhiteSpace(query.Raza))
        {
            var raza = query.Raza.Trim();
            var razaCode = raza.ToUpperInvariant();
            source = source.Where(v =>
                v.raza_code == razaCode ||
                v.raza_codeNavigation.nombre == raza);
        }

        if (!string.IsNullOrWhiteSpace(query.Procedencia))
        {
            var procedencia = query.Procedencia.Trim();
            source = source.Where(v =>
                v.granja.nombre.Contains(procedencia) ||
                v.granja.distrito_codigoNavigation.nombre.Contains(procedencia) ||
                v.granja.distrito_codigoNavigation.provincia_codigoNavigation.nombre.Contains(procedencia) ||
                v.granja.distrito_codigoNavigation.provincia_codigoNavigation.departamento_codigoNavigation.nombre.Contains(procedencia));
        }

        return source;
    }

    private IQueryable<Entities.vacuno> ApplyEstadoFilter(IQueryable<Entities.vacuno> source, string? estado)
    {
        if (string.IsNullOrWhiteSpace(estado))
        {
            return source;
        }

        return estado == "muerto"
            ? source.Where(v => _context.v_vacuno_estado_vigentes
                .Any(e => e.vacuno_id == v.id && DeadStateCodes.Contains(e.estado_code)))
            : source.Where(v => !_context.v_vacuno_estado_vigentes
                .Any(e => e.vacuno_id == v.id && DeadStateCodes.Contains(e.estado_code)));
    }

    private static IQueryable<Entities.vacuno> ApplyEstadoRegistroFilter(
        IQueryable<Entities.vacuno> source,
        string? estadoRegistro)
    {
        if (string.IsNullOrWhiteSpace(estadoRegistro))
        {
            return source;
        }

        return estadoRegistro == "eliminado"
            ? source.Where(v => v.deleted_at != null)
            : source.Where(v => v.deleted_at == null);
    }

    private IQueryable<Entities.vacuno> ApplyAptoParaFilter(IQueryable<Entities.vacuno> source, string? aptoPara)
    {
        if (string.IsNullOrWhiteSpace(aptoPara))
        {
            return source;
        }

        var aptoParaValue = aptoPara.Trim();
        var aptoParaCode = aptoParaValue.ToUpperInvariant();

        return source.Where(v => _context.v_vacuno_utilizacion_vigentes
            .Where(u => u.vacuno_id == v.id)
            .Join(
                _context.cat_tipo_utilizacions,
                u => u.tipo_utilizacion_code,
                t => t.code,
                (u, t) => new { u.tipo_utilizacion_code, t.nombre })
            .Any(u => u.tipo_utilizacion_code == aptoParaCode || u.nombre == aptoParaValue));
    }

    private Task<List<ListadoVacunosReporteProjection>> LoadPageAsync(
        IQueryable<Entities.vacuno> source,
        ListadoVacunosReporteReadQuery query,
        CancellationToken cancellationToken)
    {
        var skip = (query.Page - 1) * query.Limit;

        return source
            .OrderByDescending(v => v.fecha_registro)
            .ThenBy(v => v.codigo)
            .Skip(skip)
            .Take(query.Limit)
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
                EstadoCode = _context.v_vacuno_estado_vigentes
                    .Where(e => e.vacuno_id == v.id)
                    .Select(e => e.estado_code)
                    .FirstOrDefault(),
                DeletedAt = v.deleted_at
            })
            .ToListAsync(cancellationToken);
    }

    private static string ToEstadoBiologico(string? estadoCode)
        => DeadStateCodes.Contains((estadoCode ?? string.Empty).ToUpperInvariant()) ? "muerto" : "vivo";

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
        public DateTime? DeletedAt { get; init; }
    }
}
