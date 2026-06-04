using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReporteVacunos;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;

namespace ZooTech.Infrastructure.Persistence.Repositories;

public sealed class ReporteVacunoReadRepository : IReporteVacunoReadRepository
{
    private readonly GanaderiaDbContext _context;

    public ReporteVacunoReadRepository(GanaderiaDbContext context)
    {
        _context = context;
    }

    public async Task<ReporteVacunoListadoPage> ListarAsync(
        ReporteVacunoListadoCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        var query = _context.vacunos
            .AsNoTracking()
            .AsSplitQuery()
            .Include(vacuno => vacuno.raza_codeNavigation)
            .Include(vacuno => vacuno.granja)
                .ThenInclude(granja => granja.distrito_codigoNavigation)
                .ThenInclude(distrito => distrito.provincia_codigoNavigation)
                .ThenInclude(provincia => provincia.departamento_codigoNavigation)
            .Include(vacuno => vacuno.vacuno_estado_historials)
                .ThenInclude(historial => historial.estado_codeNavigation)
            .Include(vacuno => vacuno.vacuno_utilizacion_historials)
                .ThenInclude(historial => historial.tipo_utilizacion_codeNavigation)
            .Where(vacuno => vacuno.deleted_at == null)
            .Where(vacuno => vacuno.fecha_registro >= criteria.FechaDesde)
            .Where(vacuno => vacuno.fecha_registro <= criteria.FechaHasta);

        if (!string.IsNullOrWhiteSpace(criteria.Q))
        {
            var term = criteria.Q.Trim();
            query = query.Where(vacuno =>
                EF.Functions.Like(vacuno.codigo, $"%{term}%") ||
                EF.Functions.Like(vacuno.nombre, $"%{term}%"));
        }

        if (!string.IsNullOrWhiteSpace(criteria.Codigo))
        {
            var codigo = criteria.Codigo.Trim();
            query = query.Where(vacuno => EF.Functions.Like(vacuno.codigo, $"%{codigo}%"));
        }

        if (!string.IsNullOrWhiteSpace(criteria.Nombre))
        {
            var nombre = criteria.Nombre.Trim();
            query = query.Where(vacuno => EF.Functions.Like(vacuno.nombre, $"%{nombre}%"));
        }

        var records = await query.ToListAsync(cancellationToken);
        var filtered = records
            .Select(ToListadoItem)
            .Where(item => Matches(item.FechaRegistro, criteria.FechaRegistro))
            .Where(item => Matches(item.Raza, criteria.Raza))
            .Where(item => Matches(item.Procedencia, criteria.Procedencia))
            .Where(item => MatchesEstado(item.Estado, criteria.Estado))
            .Where(item => MatchesAptoPara(records, item.Id, criteria.AptoPara))
            .OrderByDescending(item => item.FechaRegistro)
            .ThenBy(item => item.Codigo, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var offset = (criteria.Page - 1) * criteria.Limit;
        var pageItems = filtered
            .Skip(offset)
            .Take(criteria.Limit)
            .ToList();

        return new ReporteVacunoListadoPage(pageItems, filtered.Count);
    }

    private static VacunoListadoItem ToListadoItem(vacuno vacuno)
    {
        var estado = ResolveEstado(vacuno);

        return new VacunoListadoItem(
            vacuno.id,
            FormatCodigo(vacuno.codigo),
            vacuno.fecha_registro,
            vacuno.nombre,
            vacuno.raza_codeNavigation?.nombre ?? vacuno.raza_code,
            BuildProcedencia(
                vacuno.granja?.nombre,
                vacuno.granja?.distrito_codigoNavigation?.nombre,
                vacuno.granja?.distrito_codigoNavigation?.provincia_codigoNavigation?.nombre,
                vacuno.granja?.distrito_codigoNavigation?.provincia_codigoNavigation?.departamento_codigoNavigation?.nombre),
            estado);
    }

    private static string ResolveEstado(vacuno vacuno)
    {
        var latest = vacuno.vacuno_estado_historials
            .OrderByDescending(historial => historial.id)
            .FirstOrDefault();
        var code = latest?.estado_code ?? string.Empty;
        var name = latest?.estado_codeNavigation?.nombre ?? string.Empty;
        var value = string.IsNullOrWhiteSpace(code) ? name : code;

        if (MatchesAny(value, "MUERTO", "FALLECIDO", "BAJA", "INACTIVO"))
        {
            return "muerto";
        }

        return "vivo";
    }

    private static bool MatchesAptoPara(
        IReadOnlyCollection<vacuno> records,
        long vacunoId,
        string? filter)
    {
        if (string.IsNullOrWhiteSpace(filter))
        {
            return true;
        }

        var vacuno = records.FirstOrDefault(item => item.id == vacunoId);
        var latest = vacuno?.vacuno_utilizacion_historials
            .OrderByDescending(historial => historial.id)
            .FirstOrDefault();

        return Matches(latest?.tipo_utilizacion_code, filter) ||
            Matches(latest?.tipo_utilizacion_codeNavigation?.nombre, filter);
    }

    private static bool Matches(DateOnly value, string? filter)
    {
        if (string.IsNullOrWhiteSpace(filter))
        {
            return true;
        }

        var term = filter.Trim();
        return value.ToString("yyyy-MM-dd").Contains(term, StringComparison.OrdinalIgnoreCase) ||
            value.ToString("dd/MM/yyyy").Contains(term, StringComparison.OrdinalIgnoreCase);
    }

    private static bool Matches(string? value, string? filter)
    {
        return string.IsNullOrWhiteSpace(filter) ||
            (!string.IsNullOrWhiteSpace(value) &&
             value.Contains(filter.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    private static bool MatchesEstado(string? value, string? filter)
    {
        return string.IsNullOrWhiteSpace(filter) ||
            string.Equals(value, NormalizeEstadoFilter(filter), StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeEstadoFilter(string value)
    {
        return MatchesAny(value, "MUERTO", "FALLECIDO", "BAJA", "INACTIVO")
            ? "muerto"
            : "vivo";
    }

    private static bool MatchesAny(string value, params string[] options)
    {
        return options.Any(option =>
            string.Equals(value, option, StringComparison.OrdinalIgnoreCase) ||
            value.Contains(option, StringComparison.OrdinalIgnoreCase));
    }

    private static string BuildProcedencia(params string?[] values)
    {
        return string.Join(", ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
    }

    private static string FormatCodigo(string codigo)
    {
        if (codigo.Contains('_') ||
            !codigo.StartsWith("VAC", StringComparison.OrdinalIgnoreCase) ||
            !codigo[3..].All(char.IsDigit))
        {
            return codigo;
        }

        return codigo.Insert(3, "_");
    }
}
