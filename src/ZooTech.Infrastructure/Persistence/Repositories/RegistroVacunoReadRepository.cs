using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ObtenerRegistroVacunoReporte;
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
        var rows = await _context.Set<RegistroVacunoReporteRow>()
            .FromSqlRaw(Sql, new SqlParameter("@vacuno_id", vacunoId))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var row = rows.FirstOrDefault();
        if (row is null)
        {
            return null;
        }

        return new RegistroVacunoDetalle(
            row.Id,
            row.Codigo,
            row.Nombre,
            row.FechaNacimiento,
            NormalizeCatalogValue(row.AdquisicionPor),
            row.PrecioCompra,
            row.Raza,
            row.Color,
            NormalizeCatalogValue(row.Sexo),
            row.CodigoPadre,
            row.CodigoMadre,
            row.CodigoAbuelo,
            row.CodigoAbuela,
            row.Granja,
            row.Distrito,
            row.Departamento,
            row.Provincia,
            row.Procedencia,
            NormalizeCatalogValue(row.AptoPara),
            row.FechaAdquisicion,
            row.Observaciones,
            row.FotoId,
            row.FotoNombreOriginal,
            row.FotoNombreAlmacenado,
            row.FotoRuta,
            row.FotoUrl,
            row.FotoExtension,
            row.FotoTamanoBytes,
            row.EstadoActualCode,
            row.EstadoActualNombre,
            DeterminarEstado(row.EstadoActualCode, row.EstadoActualNombre),
            row.FechaEstado,
            row.MotivoEstado,
            row.FechaRegistro,
            row.FechaAdquisicion,
            row.CreadoPor,
            row.CreadoEn,
            row.ActualizadoPor,
            row.ActualizadoEn);
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

    private const string Sql = """
SELECT
    v.id AS Id,
    v.codigo AS Codigo,
    v.nombre AS Nombre,
    v.fecha_nacimiento AS FechaNacimiento,
    sx.nombre AS Sexo,
    r.nombre AS Raza,
    c.nombre AS Color,
    ta.nombre AS AdquisicionPor,
    va.precio_compra AS PrecioCompra,
    padre.codigo AS CodigoPadre,
    madre.codigo AS CodigoMadre,
    abuelo_paterno.codigo AS CodigoAbuelo,
    abuela_materna.codigo AS CodigoAbuela,
    g.nombre AS Granja,
    gd.nombre AS Distrito,
    gdep.nombre AS Departamento,
    gp.nombre AS Provincia,
    va.proveedor AS Procedencia,
    a.id AS FotoId,
    a.nombre_original AS FotoNombreOriginal,
    a.nombre_almacenado AS FotoNombreAlmacenado,
    a.ruta_archivo AS FotoRuta,
    a.ruta_archivo AS FotoUrl,
    a.extension AS FotoExtension,
    a.tamano_bytes AS FotoTamanoBytes,
    v.observaciones AS Observaciones,
    veh.estado_code AS EstadoActualCode,
    cest.nombre AS EstadoActualNombre,
    cest.nombre AS Estado,
    veh.fecha_estado AS FechaEstado,
    veh.motivo AS MotivoEstado,
    (
        SELECT TOP(1) ctu.nombre
        FROM dbo.vacuno_utilizacion_historial vuh
        INNER JOIN dbo.cat_tipo_utilizacion ctu
            ON ctu.code = vuh.tipo_utilizacion_code
        WHERE vuh.vacuno_id = v.id
        ORDER BY vuh.created_at DESC, vuh.id DESC
    ) AS AptoPara,
    v.fecha_registro AS FechaRegistro,
    va.fecha_adquisicion AS FechaAdquisicion,
    u_created.nombre_completo AS CreadoPor,
    v.created_at AS CreadoEn,
    u_updated.nombre_completo AS ActualizadoPor,
    v.updated_at AS ActualizadoEn
FROM dbo.vacuno v
LEFT JOIN dbo.vacuno_adquisicion va
    ON va.vacuno_id = v.id
LEFT JOIN dbo.cat_tipo_adquisicion ta
    ON ta.code = v.tipo_adquisicion_code
LEFT JOIN dbo.cat_raza r
    ON r.code = v.raza_code
LEFT JOIN dbo.cat_color c
    ON c.code = v.color_code
LEFT JOIN dbo.cat_sexo sx
    ON sx.code = v.sexo_code
LEFT JOIN dbo.vacuno padre
    ON padre.id = v.padre_id
LEFT JOIN dbo.vacuno madre
    ON madre.id = v.madre_id
LEFT JOIN dbo.vacuno abuelo_paterno
    ON abuelo_paterno.id = padre.padre_id
LEFT JOIN dbo.vacuno abuela_materna
    ON abuela_materna.id = madre.madre_id
LEFT JOIN dbo.granja g
    ON g.id = v.granja_id
LEFT JOIN dbo.geo_distrito gd
    ON gd.codigo = g.distrito_codigo
LEFT JOIN dbo.geo_provincia gp
    ON gp.codigo = gd.provincia_codigo
LEFT JOIN dbo.geo_departamento gdep
    ON gdep.codigo = gp.departamento_codigo
LEFT JOIN dbo.vacuno_foto vf
    ON vf.vacuno_id = v.id
    AND vf.es_principal = 1
LEFT JOIN dbo.archivo a
    ON a.id = vf.archivo_id
LEFT JOIN dbo.vacuno_estado_historial veh
    ON veh.vacuno_id = v.id
    AND veh.id = (
        SELECT MAX(veh2.id)
        FROM dbo.vacuno_estado_historial veh2
        WHERE veh2.vacuno_id = v.id
    )
LEFT JOIN dbo.cat_estado_vacuno cest
    ON cest.code = veh.estado_code
LEFT JOIN dbo.usuario u_created
    ON u_created.id = v.created_by
LEFT JOIN dbo.usuario u_updated
    ON u_updated.id = v.updated_by
WHERE
    v.id = @vacuno_id
    AND v.deleted_at IS NULL;
""";
}
