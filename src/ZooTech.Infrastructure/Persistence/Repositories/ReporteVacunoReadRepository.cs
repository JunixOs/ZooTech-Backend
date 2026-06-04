using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReporteVacunos;
using ZooTech.Infrastructure.Persistence.Context;

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
        var offset = (criteria.Page - 1) * criteria.Limit;

        var rows = await _context.ReporteVacunoListadoRows
            .FromSqlRaw(Sql, CreateParameters(criteria, offset))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var total = rows.FirstOrDefault()?.TotalRegistros ?? 0;
        var items = rows
            .Select(row => new VacunoListadoItem(
                row.Id,
                row.Codigo,
                row.FechaRegistro,
                row.Nombre,
                row.Raza,
                row.Procedencia,
                row.Estado))
            .ToList();

        return new ReporteVacunoListadoPage(items, total);
    }

    private static object[] CreateParameters(ReporteVacunoListadoCriteria criteria, int offset)
    {
        return
        [
            new SqlParameter("@fechaDesde", System.Data.SqlDbType.Date) { Value = criteria.FechaDesde.ToDateTime(TimeOnly.MinValue) },
            new SqlParameter("@fechaHasta", System.Data.SqlDbType.Date) { Value = criteria.FechaHasta.ToDateTime(TimeOnly.MinValue) },
            new SqlParameter("@q", (object?)criteria.Q ?? DBNull.Value),
            new SqlParameter("@codigo", (object?)criteria.Codigo ?? DBNull.Value),
            new SqlParameter("@fechaRegistro", (object?)criteria.FechaRegistro ?? DBNull.Value),
            new SqlParameter("@nombre", (object?)criteria.Nombre ?? DBNull.Value),
            new SqlParameter("@raza", (object?)criteria.Raza ?? DBNull.Value),
            new SqlParameter("@procedencia", (object?)criteria.Procedencia ?? DBNull.Value),
            new SqlParameter("@estado", (object?)criteria.Estado ?? DBNull.Value),
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
        r.nombre AS Raza,
        va.proveedor AS Procedencia,
        CASE
            WHEN veh.estado_code IN ('ACTIVO', 'VIVO') THEN 'vivo'
            WHEN veh.estado_code IN ('MUERTO', 'FALLECIDO', 'BAJA') THEN 'muerto'
            ELSE LOWER(cest.nombre)
        END AS Estado,
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
            @codigo IS NULL
            OR v.codigo COLLATE Latin1_General_CI_AI LIKE '%' + @codigo + '%'
        )
        AND (
            @fechaRegistro IS NULL
            OR CONVERT(varchar(10), v.fecha_registro, 23) LIKE '%' + @fechaRegistro + '%'
            OR CONVERT(varchar(10), v.fecha_registro, 103) LIKE '%' + @fechaRegistro + '%'
        )
        AND (
            @nombre IS NULL
            OR v.nombre COLLATE Latin1_General_CI_AI LIKE '%' + @nombre + '%'
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
            OR (@estado = 'vivo' AND veh.estado_code IN ('ACTIVO', 'VIVO'))
            OR (@estado = 'muerto' AND veh.estado_code IN ('MUERTO', 'FALLECIDO', 'BAJA'))
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
    Raza,
    Procedencia,
    Estado,
    TotalRegistros
FROM filtered_vacunos
ORDER BY FechaRegistro DESC, Codigo ASC
OFFSET @offset ROWS
FETCH NEXT @limit ROWS ONLY;
""";
}
