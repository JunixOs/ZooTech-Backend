using Microsoft.EntityFrameworkCore;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;
using ZooTech.Infrastructure.Persistence.Context;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_Vacuno.Repositories;

public sealed class VacunoListadoReadRepository : IVacunoListadoReadRepository
{
    private readonly GanaderiaDbContext _context;

    public VacunoListadoReadRepository(IGanaderiaDbContextFactory ganaderiaDbContextFactory)
    {
        _context = ganaderiaDbContextFactory.CreateDbContextByTenantContext();
    }

    public async Task<IReadOnlyList<VacunoListadoReadItem>> ListarAsync(
        VacunoListadoReadQuery query,
        CancellationToken cancellationToken = default)
    {
        var source = _context.vacunos
            .AsNoTracking()
            .Select(v => new VacunoListadoProjection
            {
                Id = v.id,
                Codigo = v.codigo,
                Nombre = v.nombre,
                FechaNacimiento = v.fecha_nacimiento,
                FechaRegistro = v.fecha_registro,
                RazaCode = v.raza_code,
                SexoCode = v.sexo_code,
                Granja = v.granja.nombre,
                Distrito = v.granja.distrito_codigoNavigation.nombre,
                Provincia = v.granja.distrito_codigoNavigation.provincia_codigoNavigation.nombre,
                Departamento = v.granja.distrito_codigoNavigation.provincia_codigoNavigation.departamento_codigoNavigation.nombre,
                DeletedAt = v.deleted_at
            });

        if (!string.IsNullOrWhiteSpace(query.Q))
        {
            var term = query.Q.Trim();
            source = source.Where(v => v.Codigo.Contains(term) || v.Nombre.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(query.Estado))
        {
            var estado = query.Estado.Trim().ToLowerInvariant();
            source = estado == "eliminado"
                ? source.Where(v => v.DeletedAt != null)
                : source.Where(v => v.DeletedAt == null);
        }

        if (query.FechaDesde.HasValue)
        {
            source = source.Where(v => v.FechaRegistro >= query.FechaDesde.Value);
        }

        if (query.FechaHasta.HasValue)
        {
            source = source.Where(v => v.FechaRegistro <= query.FechaHasta.Value);
        }

        var rows = await source
            .OrderBy(v => v.Codigo)
            .ToListAsync(cancellationToken);

        return rows
            .Select(v => new VacunoListadoReadItem(
                v.Id,
                v.Codigo,
                v.Nombre,
                v.FechaNacimiento,
                v.FechaRegistro,
                v.RazaCode,
                v.SexoCode,
                BuildProcedencia(v.Granja, v.Distrito, v.Provincia, v.Departamento),
                v.DeletedAt.HasValue))
            .ToList();
    }

    private static string? BuildProcedencia(params string?[] values)
    {
        var parts = values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value!.Trim());

        return string.Join(", ", parts);
    }

    private sealed class VacunoListadoProjection
    {
        public long Id { get; init; }
        public string Codigo { get; init; } = string.Empty;
        public string Nombre { get; init; } = string.Empty;
        public DateOnly FechaNacimiento { get; init; }
        public DateOnly FechaRegistro { get; init; }
        public string RazaCode { get; init; } = string.Empty;
        public string SexoCode { get; init; } = string.Empty;
        public string? Granja { get; init; }
        public string? Distrito { get; init; }
        public string? Provincia { get; init; }
        public string? Departamento { get; init; }
        public DateTime? DeletedAt { get; init; }
    }
}
