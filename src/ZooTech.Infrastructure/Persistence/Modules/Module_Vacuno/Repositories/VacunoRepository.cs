using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ZooTech.Domain.Module_Vacuno.Entities;
using ZooTech.Domain.Module_Vacuno.Interfaces;
using ZooTech.Infrastructure.Persistence.Context;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_Vacuno.Repositories;

public sealed class VacunoRepository : IVacunoRepository
{
    private readonly GanaderiaDbContext _context;

    public VacunoRepository(GanaderiaDbContext context)
    {
        _context = context;
    }

    public async Task<List<Vacuno>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.vacunos
            .AsNoTracking()
            .Where(v => v.deleted_at == null)
            .OrderBy(v => v.codigo)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain).ToList();
    }

    public async Task<List<(Vacuno Vacuno, string? Procedencia)>> ListAllForDisplayAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.vacunos
            .AsNoTracking()
            .Include(v => v.granja)
                .ThenInclude(g => g.distrito_codigoNavigation)
                    .ThenInclude(d => d.provincia_codigoNavigation)
                        .ThenInclude(p => p.departamento_codigoNavigation)
            .Where(v => v.deleted_at == null)
            .OrderBy(v => v.codigo)
            .ToListAsync(cancellationToken);

        return entities.Select(v =>
        {
            string? procedencia = null;
            var g = v.granja;
            if (g is not null)
            {
                var d = g.distrito_codigoNavigation;
                var p = d?.provincia_codigoNavigation;
                var dep = p?.departamento_codigoNavigation;
                procedencia = string.Join(", ",
                    new[] { g.nombre, d?.nombre, p?.nombre, dep?.nombre }
                    .Where(s => !string.IsNullOrWhiteSpace(s)));
            }
            return (ToDomain(v), procedencia);
        }).ToList();
    }

    public async Task<Vacuno?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.vacunos
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.id == id && v.deleted_at == null, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.vacunos
            .AnyAsync(v => v.id == id && v.deleted_at == null, cancellationToken);
    }

    public Task<bool> ExistsCodigoAsync(string codigo, CancellationToken cancellationToken = default)
        => _context.vacunos.AnyAsync(v => v.deleted_at == null && v.codigo == codigo.Trim(), cancellationToken);

    public async Task<Vacuno> AddAsync(Vacuno vacuno, CancellationToken cancellationToken = default)
    {
        var entity = ToEntity(vacuno);
        _context.vacunos.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return ToDomain(entity);
    }

    public async Task<Vacuno> UpdateAsync(Vacuno vacuno, CancellationToken cancellationToken = default)
    {
        var entity = await _context.vacunos
            .FirstOrDefaultAsync(v => v.id == vacuno.Id, cancellationToken)
            ?? throw new InvalidOperationException("No se encontró el vacuno para actualizar.");

        entity.nombre = vacuno.Nombre;
        entity.fecha_nacimiento = vacuno.FechaNacimiento;
        entity.tipo_adquisicion_code = vacuno.TipoAdquisicionCode;
        entity.raza_code = vacuno.RazaCode;
        entity.color_code = vacuno.ColorCode;
        entity.sexo_code = vacuno.SexoCode;
        entity.padre_id = vacuno.PadreId;
        entity.madre_id = vacuno.MadreId;
        entity.granja_id = vacuno.GranjaId;
        entity.observaciones = vacuno.Observaciones;
        entity.updated_by = vacuno.UpdatedBy;
        entity.updated_at = vacuno.UpdatedAt;
        entity.deleted_at = vacuno.DeletedAt;
        entity.deleted_by = vacuno.DeletedBy;
        entity.motivo_eliminacion = vacuno.MotivoEliminacion;

        await _context.SaveChangesAsync(cancellationToken);
        return ToDomain(entity);
    }

    private static Entities.vacuno ToEntity(Vacuno domain)
        => new()
        {
            codigo = domain.Codigo,
            nombre = domain.Nombre,
            fecha_nacimiento = domain.FechaNacimiento,
            tipo_adquisicion_code = domain.TipoAdquisicionCode,
            raza_code = domain.RazaCode,
            color_code = domain.ColorCode,
            sexo_code = domain.SexoCode,
            padre_id = domain.PadreId,
            madre_id = domain.MadreId,
            granja_id = domain.GranjaId,
            observaciones = domain.Observaciones,
            fecha_registro = domain.FechaRegistro,
            created_by = domain.CreatedBy,
            updated_by = domain.UpdatedBy,
            deleted_by = domain.DeletedBy,
            created_at = domain.CreatedAt,
            updated_at = domain.UpdatedAt,
            deleted_at = domain.DeletedAt,
            motivo_eliminacion = domain.MotivoEliminacion
        };

    public async Task<(IReadOnlyCollection<VacunoListadoItemDomain> Items, int TotalRegistros)> ListarAvanzadoAsync(
        ListarVacunosCriteriaDomain criteria,
        CancellationToken cancellationToken = default)
    {
        var offset = (criteria.Page - 1) * criteria.Limit;

        var rows = await _context.ReporteVacunoListadoRows
            .FromSqlRaw(Sql, CreateParameters(criteria, offset))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var total = rows.FirstOrDefault()?.TotalRegistros ?? 0;
        var items = rows
            .Select(row => new VacunoListadoItemDomain(
                row.Id,
                row.Codigo,
                row.FechaRegistro,
                row.Nombre,
                row.FechaNacimiento,
                row.RazaCode,
                row.SexoCode,
                row.Raza,
                row.Procedencia,
                DeterminarEstado(row.EstadoActualCode, row.EstadoActualNombre)))
            .ToList();

        return (items, total);
    }

    private static string? NormalizeCatalogValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

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

    private static object[] CreateParameters(ListarVacunosCriteriaDomain criteria, int offset)
    {
        string estadoCodes = string.Empty;
        var estado = criteria.Estado?.ToLowerInvariant();
        
        if (estado == "vivo") estadoCodes = "ACTIVO,VIVO";
        else if (estado == "muerto") estadoCodes = "MUERTO,FALLECIDO,BAJA";

        return
        [
            new SqlParameter("@fechaDesde", System.Data.SqlDbType.Date) { Value = criteria.FechaDesde.ToDateTime(TimeOnly.MinValue) },
            new SqlParameter("@fechaHasta", System.Data.SqlDbType.Date) { Value = criteria.FechaHasta.ToDateTime(TimeOnly.MinValue) },
            new SqlParameter("@q", (object?)criteria.Q ?? DBNull.Value),
            new SqlParameter("@raza", (object?)criteria.Raza ?? DBNull.Value),
            new SqlParameter("@procedencia", (object?)criteria.Procedencia ?? DBNull.Value),
            new SqlParameter("@estado", (object?)criteria.Estado ?? DBNull.Value),
            new SqlParameter("@estadoCodes", estadoCodes),
            new SqlParameter("@aptoPara", (object?)criteria.AptoPara ?? DBNull.Value),
            new SqlParameter("@offset", offset),
            new SqlParameter("@limit", criteria.Limit)
        ];
    }

    private const string Sql = """
WITH filtered_vacunos AS (
    SELECT
        v.id AS Id,
        v.codigo AS Codigo,
        v.fecha_registro AS FechaRegistro,
        v.nombre AS Nombre,
        v.fecha_nacimiento AS FechaNacimiento,
        v.raza_code AS RazaCode,
        v.sexo_code AS SexoCode,
        r.nombre AS Raza,
        va.proveedor AS Procedencia,
        veh.estado_code AS EstadoActualCode,
        cest.nombre AS EstadoActualNombre,
        COUNT(*) OVER() AS TotalRegistros
    FROM dbo.vacuno v
    LEFT JOIN dbo.cat_raza r ON r.code = v.raza_code
    LEFT JOIN dbo.vacuno_adquisicion va ON va.vacuno_id = v.id
    LEFT JOIN dbo.vacuno_estado_historial veh ON veh.vacuno_id = v.id
        AND veh.id = (
            SELECT MAX(veh2.id)
            FROM dbo.vacuno_estado_historial veh2
            WHERE veh2.vacuno_id = v.id
        )
    LEFT JOIN dbo.cat_estado_vacuno cest ON cest.code = veh.estado_code
    LEFT JOIN dbo.vacuno_utilizacion_historial vuh_latest ON vuh_latest.vacuno_id = v.id
        AND vuh_latest.id = (
            SELECT MAX(vuh2.id)
            FROM dbo.vacuno_utilizacion_historial vuh2
            WHERE vuh2.vacuno_id = v.id
        )
    LEFT JOIN dbo.cat_tipo_utilizacion ctu ON ctu.code = vuh_latest.tipo_utilizacion_code
    WHERE
        v.deleted_at IS NULL
        AND v.fecha_registro >= @fechaDesde
        AND v.fecha_registro < DATEADD(DAY, 1, @fechaHasta)
        AND (
            @q IS NULL
            OR v.codigo COLLATE Latin1_General_CI_AI LIKE '%' + @q + '%'
            OR v.nombre COLLATE Latin1_General_CI_AI LIKE '%' + @q + '%'
        )
        AND (
            @raza IS NULL
            OR r.nombre COLLATE Latin1_General_CI_AI LIKE '%' + @raza + '%'
        )
        AND (
            @procedencia IS NULL
            OR va.proveedor COLLATE Latin1_General_CI_AI LIKE '%' + @procedencia + '%'
        )
        AND (
            @estado IS NULL
            OR veh.estado_code COLLATE Latin1_General_CI_AI = @estado COLLATE Latin1_General_CI_AI
            OR cest.nombre COLLATE Latin1_General_CI_AI = @estado COLLATE Latin1_General_CI_AI
            OR (@estadoCodes != '' AND veh.estado_code IN (SELECT value FROM STRING_SPLIT(@estadoCodes, ',')))
        )
        AND (
            @aptoPara IS NULL
            OR ctu.code COLLATE Latin1_General_CI_AI = @aptoPara COLLATE Latin1_General_CI_AI
            OR ctu.nombre COLLATE Latin1_General_CI_AI LIKE '%' + @aptoPara + '%'
        )
)
SELECT
    Id,
    Codigo,
    FechaRegistro,
    Nombre,
    FechaNacimiento,
    RazaCode,
    SexoCode,
    Raza,
    Procedencia,
    EstadoActualCode,
    EstadoActualNombre,
    TotalRegistros
FROM filtered_vacunos
ORDER BY FechaRegistro DESC, Codigo ASC
OFFSET @offset ROWS
FETCH NEXT @limit ROWS ONLY;
""";

    private static Vacuno ToDomain(Entities.vacuno entity)
    {
        return Vacuno.Rehydrate(
            id: entity.id,
            codigo: entity.codigo,
            nombre: entity.nombre,
            fechaNacimiento: entity.fecha_nacimiento,
            tipoAdquisicionCode: entity.tipo_adquisicion_code,
            razaCode: entity.raza_code,
            colorCode: entity.color_code,
            sexoCode: entity.sexo_code,
            padreId: entity.padre_id,
            madreId: entity.madre_id,
            granjaId: entity.granja_id,
            observaciones: entity.observaciones,
            fechaRegistro: entity.fecha_registro,
            createdAt: entity.created_at,
            updatedAt: entity.updated_at,
            deletedAt: entity.deleted_at,
            motivoEliminacion: entity.motivo_eliminacion,
            createdBy: entity.created_by,
            updatedBy: entity.updated_by,
            deletedBy: entity.deleted_by);
    }
}
