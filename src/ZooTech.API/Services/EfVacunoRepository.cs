using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ZooTech.API.Models;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;
using System.Linq;

namespace ZooTech.API.Services;

public sealed class EfVacunoRepository : IVacunoRepository
{
    private readonly GanaderiaDbContext _db;

    public EfVacunoRepository(GanaderiaDbContext db)
    {
        _db = db;
    }

    public PagedResponse<VacunoResponse> Search(
        string? search,
        int page,
        int pageSize,
        DateOnly? fechaDesde = null,
        DateOnly? fechaHasta = null,
        string? estado = null)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _db.vacunos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(v =>
                EF.Functions.Like(v.codigo, $"%{term}%") ||
                EF.Functions.Like(v.nombre, $"%{term}%") ||
                EF.Functions.Like(v.raza_code, $"%{term}%") ||
                EF.Functions.Like(v.color_code, $"%{term}%") ||
                EF.Functions.Like(v.sexo_code, $"%{term}%") ||
                EF.Functions.Like(v.granja.nombre, $"%{term}%"));
        }

        if (fechaDesde is not null)
        {
            query = query.Where(v => v.fecha_registro >= fechaDesde.Value);
        }

        if (fechaHasta is not null)
        {
            query = query.Where(v => v.fecha_registro <= fechaHasta.Value);
        }

        if (!string.IsNullOrWhiteSpace(estado))
        {
            if (string.Equals(estado, "eliminado", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(v => v.deleted_at != null);
            }
            else if (string.Equals(estado, "activo", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(v => v.deleted_at == null);
            }
        }

        var totalItems = query.Count();
        var entities = query
            .Include(x => x.granja)
                .ThenInclude(g => g.distrito_codigoNavigation)
                .ThenInclude(d => d.provincia_codigoNavigation)
                .ThenInclude(p => p.departamento_codigoNavigation)
            .Include(x => x.padre)
            .Include(x => x.madre)
            .OrderBy(v => v.codigo)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var items = entities.Select(v => new VacunoResponse(
                Codigo: FormatCodigo(v.codigo),
                Nombre: v.nombre,
                FechaNacimiento: v.fecha_nacimiento.ToString("yyyy-MM-dd"),
                AdquisicionPor: v.tipo_adquisicion_code,
                Raza: v.raza_code,
                Color: v.color_code,
                Sexo: v.sexo_code,
                CodigoPadre: v.padre?.codigo ?? string.Empty,
                CodigoMadre: v.madre?.codigo ?? string.Empty,
                Granja: v.granja?.nombre ?? string.Empty,
                Distrito: v.granja?.distrito_codigoNavigation?.nombre ?? string.Empty,
                Departamento: v.granja?.distrito_codigoNavigation?.provincia_codigoNavigation?.departamento_codigoNavigation?.nombre ?? string.Empty,
                Provincia: v.granja?.distrito_codigoNavigation?.provincia_codigoNavigation?.nombre ?? string.Empty,
                AptoPara: GetLatestTipoUtilizacion(v.id),
                FechaRegistroFuncion: v.fecha_registro.ToString("yyyy-MM-dd"),
                Observaciones: v.observaciones ?? string.Empty,
                FotoUrl: string.Empty,
                Estado: v.deleted_at == null ? "activo" : "eliminado"
            ))
            .ToList();

        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)pageSize);

        return new PagedResponse<VacunoResponse>(items, page, pageSize, totalItems, totalPages);
    }

    public VacunoResponse? GetByCodigo(string codigo, VerVacunoParametros? parametros = null)
    {
        var reglas = parametros ?? new VerVacunoParametros();
        var codigoBuscado = codigo.Trim();
        var codigoSinSeparadores = codigoBuscado.Replace("_", "");
        var query = _db.vacunos
            .Include(x => x.granja)
                .ThenInclude(g => g.distrito_codigoNavigation)
                .ThenInclude(d => d.provincia_codigoNavigation)
                .ThenInclude(p => p.departamento_codigoNavigation)
            .Include(x => x.padre)
            .Include(x => x.madre)
            .AsQueryable();

        if (reglas.PermitirBusquedaCodigoSinSeparadores)
        {
            query = query.Where(x => x.codigo == codigoBuscado || x.codigo.Replace("_", "") == codigoSinSeparadores);
        }
        else
        {
            query = query.Where(x => x.codigo == codigoBuscado);
        }

        if (reglas.ExcluirEliminados)
        {
            query = query.Where(x => x.deleted_at == null);
        }

        var v = query.FirstOrDefault();

        if (v == null) return null;

        return new VacunoResponse(
            Codigo: FormatCodigo(v.codigo),
            Nombre: v.nombre,
            FechaNacimiento: v.fecha_nacimiento.ToString("yyyy-MM-dd"),
            AdquisicionPor: v.tipo_adquisicion_code,
            Raza: v.raza_code,
            Color: v.color_code,
            Sexo: v.sexo_code,
            CodigoPadre: v.padre?.codigo ?? string.Empty,
            CodigoMadre: v.madre?.codigo ?? string.Empty,
            Granja: v.granja.nombre,
            Distrito: v.granja.distrito_codigoNavigation?.nombre ?? string.Empty,
            Departamento: v.granja.distrito_codigoNavigation?.provincia_codigoNavigation?.departamento_codigoNavigation?.nombre ?? string.Empty,
            Provincia: v.granja.distrito_codigoNavigation?.provincia_codigoNavigation?.nombre ?? string.Empty,
            AptoPara: GetLatestTipoUtilizacion(v.id),
            FechaRegistroFuncion: v.fecha_registro.ToString("yyyy-MM-dd"),
            Observaciones: v.observaciones ?? string.Empty,
            FotoUrl: string.Empty,
            Estado: v.deleted_at == null ? "activo" : "eliminado"
        );
    }

    public VacunoResponse Create(RegistrarVacunoRequest request)
    {
        var codigo = request.Codigo.Trim().ToUpperInvariant();
        var codigoKey = codigo.Replace("_", "");

        if (_db.vacunos.Any(v =>
                (v.codigo == codigo || v.codigo.Replace("_", "") == codigoKey) &&
                v.deleted_at == null))
        {
            throw new InvalidOperationException("VACUNO_ALREADY_EXISTS");
        }

        var now = DateTime.UtcNow;
        var today = DateOnly.FromDateTime(now);
        var granja = ResolveGranja(
            request.Granja,
            request.CodigoDistrito,
            request.CodigoProvincia,
            request.CodigoDepartamento,
            request.Distrito,
            request.Provincia,
            request.Departamento);

        if (granja is null)
        {
            throw new InvalidOperationException("No existe una granja registrada para asociar el vacuno.");
        }

        var padre = string.IsNullOrWhiteSpace(request.CodigoPadre)
            ? null
            : FindByNormalizedCodigo(request.CodigoPadre);
        var madre = string.IsNullOrWhiteSpace(request.CodigoMadre)
            ? null
            : FindByNormalizedCodigo(request.CodigoMadre);

        var entity = new vacuno
        {
            codigo = codigo,
            nombre = request.Nombre.Trim(),
            fecha_nacimiento = request.FechaNacimiento,
            tipo_adquisicion_code = ResolveTipoAdquisicionCode(request.AdquisicionPor),
            raza_code = ResolveRazaCode(request.Raza),
            color_code = ResolveColorCode(request.Color),
            sexo_code = ResolveSexoCode(request.Sexo),
            padre_id = padre?.id,
            madre_id = madre?.id,
            granja_id = granja.id,
            observaciones = request.Observaciones,
            fecha_registro = request.FechaEspecificacion == default ? today : request.FechaEspecificacion,
            created_at = now,
            updated_at = now
        };

        _db.vacunos.Add(entity);
        _db.SaveChanges();

        var adquisicion = new vacuno_adquisicion
        {
            vacuno_id = entity.id,
            tipo_adquisicion_code = entity.tipo_adquisicion_code,
            precio_compra = request.PrecioCompra,
            fecha_adquisicion = today,
            created_at = now
        };
        var utilizacion = new vacuno_utilizacion_historial
        {
            vacuno_id = entity.id,
            tipo_utilizacion_code = ResolveTipoUtilizacionCode(request.AptoPara),
            motivo = request.Observaciones,
            created_at = now
        };

        _db.vacuno_adquisicions.Add(adquisicion);
        _db.vacuno_utilizacion_historials.Add(utilizacion);
        _db.SaveChanges();

        return GetByCodigo(codigo) ?? new VacunoResponse(
            Codigo: codigo,
            Nombre: request.Nombre.Trim(),
            FechaNacimiento: request.FechaNacimiento.ToString("yyyy-MM-dd"),
            AdquisicionPor: request.AdquisicionPor,
            Raza: request.Raza,
            Color: request.Color,
            Sexo: request.Sexo,
            CodigoPadre: request.CodigoPadre,
            CodigoMadre: request.CodigoMadre,
            Granja: request.Granja,
            Distrito: request.Distrito,
            Departamento: request.Departamento,
            Provincia: request.Provincia,
            AptoPara: request.AptoPara,
            FechaRegistroFuncion: request.FechaEspecificacion.ToString("yyyy-MM-dd"),
            Observaciones: request.Observaciones ?? string.Empty,
            FotoUrl: request.FotoUrl ?? string.Empty,
            Estado: "activo");
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

    private vacuno? FindByNormalizedCodigo(string codigo)
    {
        var normalized = codigo.Trim().ToUpperInvariant();
        var key = normalized.Replace("_", "");

        return _db.vacunos.FirstOrDefault(v =>
            (v.codigo == normalized || v.codigo.Replace("_", "") == key) &&
            v.deleted_at == null);
    }

    private string ResolveTipoAdquisicionCode(string value) =>
        _db.cat_tipo_adquisicions
            .Where(c => c.code == value || c.nombre == value)
            .Select(c => c.code)
            .FirstOrDefault() ??
        _db.cat_tipo_adquisicions.Select(c => c.code).FirstOrDefault() ??
        value;

    private string ResolveRazaCode(string value) =>
        _db.cat_razas
            .Where(c => c.code == value || c.nombre == value)
            .Select(c => c.code)
            .FirstOrDefault() ??
        _db.cat_razas.Select(c => c.code).FirstOrDefault() ??
        value;

    private string ResolveColorCode(string value) =>
        _db.cat_colors
            .Where(c => c.code == value || c.nombre == value)
            .Select(c => c.code)
            .FirstOrDefault() ??
        _db.cat_colors.Select(c => c.code).FirstOrDefault() ??
        value;

    private string ResolveSexoCode(string value) =>
        _db.cat_sexos
            .Where(c => c.code == value || c.nombre == value)
            .Select(c => c.code)
            .FirstOrDefault() ??
        _db.cat_sexos.Select(c => c.code).FirstOrDefault() ??
        value;

    private string ResolveTipoUtilizacionCode(string value) =>
        _db.cat_tipo_utilizacions
            .Where(c => c.code == value || c.nombre == value)
            .Select(c => c.code)
            .FirstOrDefault() ??
        _db.cat_tipo_utilizacions.Select(c => c.code).FirstOrDefault() ??
        value;

    private string GetLatestTipoUtilizacion(long vacunoId) =>
        _db.vacuno_utilizacion_historials
            .Where(u => u.vacuno_id == vacunoId)
            .OrderByDescending(u => u.created_at)
            .Select(u => u.tipo_utilizacion_code)
            .FirstOrDefault() ?? string.Empty;

    public VacunoResponse? Update(string codigo, UpdateVacunoRequest request)
    {
        var searchKey = codigo.Replace("_", "");
        var v = _db.vacunos.FirstOrDefault(x =>
            (x.codigo == codigo || x.codigo.Replace("_", "") == searchKey) &&
            x.deleted_at == null);
        if (v == null) return null;

        var granjaNombre = request.Granja.Trim();
        var granja = ResolveGranja(
            granjaNombre,
            request.CodigoDistrito,
            request.CodigoProvincia,
            request.CodigoDepartamento,
            request.Distrito,
            request.Provincia,
            request.Departamento);
        if (granja is null)
        {
            throw new InvalidOperationException("GRANJA_NOT_FOUND");
        }

        var padre = string.IsNullOrWhiteSpace(request.CodigoPadre)
            ? null
            : FindByNormalizedCodigo(request.CodigoPadre);
        var madre = string.IsNullOrWhiteSpace(request.CodigoMadre)
            ? null
            : FindByNormalizedCodigo(request.CodigoMadre);
        var now = DateTime.UtcNow;

        v.nombre = request.Nombre.Trim();
        v.fecha_nacimiento = DateOnly.Parse(request.FechaNacimiento);
        v.tipo_adquisicion_code = ResolveTipoAdquisicionCode(request.AdquisicionPor);
        v.raza_code = ResolveRazaCode(request.Raza);
        v.color_code = ResolveColorCode(request.Color);
        v.sexo_code = ResolveSexoCode(request.Sexo);
        v.padre_id = padre?.id;
        v.madre_id = madre?.id;
        v.granja_id = granja.id;
        v.fecha_registro = string.IsNullOrWhiteSpace(request.FechaRegistroFuncion)
            ? v.fecha_registro
            : DateOnly.Parse(request.FechaRegistroFuncion);
        v.observaciones = string.IsNullOrWhiteSpace(request.Observaciones)
            ? null
            : request.Observaciones.Trim();
        v.updated_at = now;

        var adquisicion = _db.vacuno_adquisicions.FirstOrDefault(a => a.vacuno_id == v.id);
        if (adquisicion is null)
        {
            _db.vacuno_adquisicions.Add(new vacuno_adquisicion
            {
                vacuno_id = v.id,
                tipo_adquisicion_code = v.tipo_adquisicion_code,
                fecha_adquisicion = DateOnly.FromDateTime(now),
                created_at = now
            });
        }
        else
        {
            adquisicion.tipo_adquisicion_code = v.tipo_adquisicion_code;
        }

        if (!string.IsNullOrWhiteSpace(request.AptoPara))
        {
            var utilizacionCode = ResolveTipoUtilizacionCode(request.AptoPara);
            var utilizacion = _db.vacuno_utilizacion_historials
                .Where(u => u.vacuno_id == v.id)
                .OrderByDescending(u => u.created_at)
                .FirstOrDefault();

            if (utilizacion is null)
            {
                _db.vacuno_utilizacion_historials.Add(new vacuno_utilizacion_historial
                {
                    vacuno_id = v.id,
                    tipo_utilizacion_code = utilizacionCode,
                    motivo = v.observaciones,
                    created_at = now
                });
            }
            else
            {
                utilizacion.tipo_utilizacion_code = utilizacionCode;
                utilizacion.motivo = v.observaciones;
            }
        }

        _db.SaveChanges();

        return GetByCodigo(v.codigo);
    }

    public DeleteVacunoResponse? Delete(string codigo, string motivoEliminacion)
    {
        var searchKey = codigo.Replace("_", "");
        var v = _db.vacunos.FirstOrDefault(x =>
            (x.codigo == codigo || x.codigo.Replace("_", "") == searchKey) &&
            x.deleted_at == null);
        if (v == null) return null;

        var fechaEliminacion = DateTime.UtcNow;
        v.deleted_at = fechaEliminacion;
        v.motivo_eliminacion = motivoEliminacion;
        v.updated_at = fechaEliminacion;
        _db.SaveChanges();

        return new DeleteVacunoResponse(
            Codigo: FormatCodigo(v.codigo),
            Nombre: v.nombre,
            MotivoEliminacion: motivoEliminacion,
            FechaEliminacion: fechaEliminacion);
    }

    public VacunoOptionsResponse GetOptions()
    {
        var adquis = _db.vacunos.Select(v => v.tipo_adquisicion_code).Distinct().ToList();
        var razas = _db.vacunos.Select(v => v.raza_code).Distinct().ToList();
        var apto = new List<string>();

        return new VacunoOptionsResponse(
            adquis,
            razas,
            new List<string> { "Hembra", "Macho" },
            apto,
            GetUbigeoOptions());
    }

    private IReadOnlyList<UbigeoOptionResponse> GetUbigeoOptions()
    {
        try
        {
            var departamentos = _db.geo_departamentos
                .AsNoTracking()
                .OrderBy(departamento => departamento.nombre)
                .Select(departamento => new
                {
                    departamento.codigo,
                    departamento.nombre
                })
                .ToList();

            var provincias = _db.geo_provincia
                .AsNoTracking()
                .OrderBy(provincia => provincia.nombre)
                .Select(provincia => new
                {
                    provincia.codigo,
                    provincia.nombre,
                    provincia.departamento_codigo
                })
                .ToList();

            var distritos = _db.geo_distritos
                .AsNoTracking()
                .OrderBy(distrito => distrito.nombre)
                .Select(distrito => new
                {
                    distrito.codigo,
                    distrito.nombre,
                    distrito.provincia_codigo
                })
                .ToList();

            return departamentos
                .Select(departamento => new UbigeoOptionResponse(
                    departamento.codigo,
                    departamento.nombre,
                    provincias
                        .Where(provincia => provincia.departamento_codigo == departamento.codigo)
                        .Select(provincia => new UbigeoOptionResponse(
                            provincia.codigo,
                            provincia.nombre,
                            distritos
                                .Where(distrito => distrito.provincia_codigo == provincia.codigo)
                                .Select(distrito => new UbigeoOptionResponse(distrito.codigo, distrito.nombre, []))
                                .ToList()))
                        .ToList()))
                .ToList();
        }
        catch (SqlException exception) when (exception.Number == 208)
        {
            return [];
        }
    }

    public VacunoActividadStatsResponse GetActivityStats(
        DateOnly fechaInicio,
        DateOnly fechaFin,
        VacunoActividadParametros? parametros = null)
    {
        var reglas = parametros ?? new VacunoActividadParametros();
        var records = _db.vacunos
            .AsNoTracking()
            .Select(v => new
            {
                FechaRegistro = v.fecha_registro,
                v.deleted_at
            })
            .ToList();

        var points = EnumerateDates(fechaInicio, fechaFin)
            .Select(fecha => new VacunoActividadPointResponse(
                Fecha: fecha,
                Cantidad: records.Count(v => IsActiveForStats(
                    v.FechaRegistro,
                    v.deleted_at,
                    fecha,
                    reglas))))
            .ToList();

        var quantities = points.Select(point => point.Cantidad).ToList();

        return new VacunoActividadStatsResponse(
            FechaInicio: fechaInicio,
            FechaFin: fechaFin,
            Points: points,
            Mayor: quantities.Count == 0 ? 0 : quantities.Max(),
            Menor: quantities.Count == 0 ? 0 : quantities.Min());
    }

    public VacunoGenealogiaResponse? GetGenealogia(string codigoOId, int niveles)
    {
        var maxNiveles = Math.Clamp(niveles, 1, 4);
        var root = ResolveGenealogyRoot(codigoOId);

        return root is null
            ? null
            : BuildGenealogyNode(root, nivelActual: 0, maxNiveles, new HashSet<long>());
    }

    private static IEnumerable<DateOnly> EnumerateDates(DateOnly fechaInicio, DateOnly fechaFin)
    {
        for (var current = fechaInicio; current <= fechaFin; current = current.AddDays(1))
        {
            yield return current;
        }
    }

    private granja? ResolveGranja(
        string granjaNombre,
        string? codigoDistrito,
        string? codigoProvincia,
        string? codigoDepartamento,
        string? distrito,
        string? provincia,
        string? departamento)
    {
        var nombre = granjaNombre.Trim();
        var query = _db.granjas
            .Include(g => g.distrito_codigoNavigation)
                .ThenInclude(d => d.provincia_codigoNavigation)
                .ThenInclude(p => p.departamento_codigoNavigation)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(nombre))
        {
            query = query.Where(g => g.nombre == nombre || g.nombre.ToLower() == nombre.ToLower());
        }

        if (!string.IsNullOrWhiteSpace(codigoDistrito))
        {
            query = query.Where(g => g.distrito_codigo == codigoDistrito.Trim());
        }
        else if (!string.IsNullOrWhiteSpace(distrito))
        {
            var distritoNormalizado = distrito.Trim().ToLower();
            query = query.Where(g => g.distrito_codigoNavigation.nombre.ToLower() == distritoNormalizado);
        }

        if (!string.IsNullOrWhiteSpace(codigoProvincia))
        {
            query = query.Where(g => g.distrito_codigoNavigation.provincia_codigo == codigoProvincia.Trim());
        }
        else if (!string.IsNullOrWhiteSpace(provincia))
        {
            var provinciaNormalizada = provincia.Trim().ToLower();
            query = query.Where(g => g.distrito_codigoNavigation.provincia_codigoNavigation.nombre.ToLower() == provinciaNormalizada);
        }

        if (!string.IsNullOrWhiteSpace(codigoDepartamento))
        {
            query = query.Where(g => g.distrito_codigoNavigation.provincia_codigoNavigation.departamento_codigo == codigoDepartamento.Trim());
        }
        else if (!string.IsNullOrWhiteSpace(departamento))
        {
            var departamentoNormalizado = departamento.Trim().ToLower();
            query = query.Where(g => g.distrito_codigoNavigation.provincia_codigoNavigation.departamento_codigoNavigation.nombre.ToLower() == departamentoNormalizado);
        }

        return query.FirstOrDefault() ??
            _db.granjas.FirstOrDefault(g => g.nombre == nombre) ??
            _db.granjas.FirstOrDefault(g => g.nombre.ToLower() == nombre.ToLower()) ??
            _db.granjas.FirstOrDefault();
    }

    private static bool IsActiveForStats(
        DateOnly fechaRegistro,
        DateTime? fechaEliminacion,
        DateOnly fecha,
        VacunoActividadParametros parametros)
    {
        if (fechaRegistro > fecha)
        {
            return false;
        }

        if (!parametros.ContarEliminadosHastaFechaEliminacion)
        {
            return fechaEliminacion == null;
        }

        return fechaEliminacion == null || DateOnly.FromDateTime(fechaEliminacion.Value) > fecha;
    }

    private GenealogyVacuno? ResolveGenealogyRoot(string codigoOId)
    {
        var value = codigoOId.Trim();

        if (long.TryParse(value, out var id))
        {
            return GetGenealogyVacunoById(id);
        }

        var codigoSinSeparadores = value.Replace("_", "");

        var entity = _db.vacunos
            .AsNoTracking()
            .Include(v => v.granja)
                .ThenInclude(g => g.distrito_codigoNavigation)
                .ThenInclude(d => d.provincia_codigoNavigation)
                .ThenInclude(p => p.departamento_codigoNavigation)
            .Include(v => v.raza_codeNavigation)
            .Include(v => v.sexo_codeNavigation)
            .Where(v =>
                (v.codigo == value || v.codigo.Replace("_", "") == codigoSinSeparadores) &&
                v.deleted_at == null)
            .FirstOrDefault();

        return entity is null ? null : ToGenealogyVacuno(entity);
    }

    private GenealogyVacuno? GetGenealogyVacunoById(long id)
    {
        var entity = _db.vacunos
            .AsNoTracking()
            .Include(v => v.granja)
                .ThenInclude(g => g.distrito_codigoNavigation)
                .ThenInclude(d => d.provincia_codigoNavigation)
                .ThenInclude(p => p.departamento_codigoNavigation)
            .Include(v => v.raza_codeNavigation)
            .Include(v => v.sexo_codeNavigation)
            .Where(v => v.id == id && v.deleted_at == null)
            .FirstOrDefault();

        return entity is null ? null : ToGenealogyVacuno(entity);
    }

    private VacunoGenealogiaResponse BuildGenealogyNode(
        GenealogyVacuno vacuno,
        int nivelActual,
        int maxNiveles,
        HashSet<long> visited)
    {
        visited.Add(vacuno.Id);

        return new VacunoGenealogiaResponse(
            vacuno.Id,
            vacuno.Codigo,
            vacuno.Nombre,
            vacuno.Raza,
            vacuno.Procedencia,
            vacuno.Sexo,
            nivelActual,
            ResolveParentNode(vacuno.PadreId, nivelActual, maxNiveles, visited),
            ResolveParentNode(vacuno.MadreId, nivelActual, maxNiveles, visited));
    }

    private VacunoGenealogiaResponse? ResolveParentNode(
        long? parentId,
        int nivelActual,
        int maxNiveles,
        HashSet<long> visited)
    {
        if (parentId is null || nivelActual >= maxNiveles || visited.Contains(parentId.Value))
        {
            return null;
        }

        var parent = GetGenealogyVacunoById(parentId.Value);

        return parent is null
            ? null
            : BuildGenealogyNode(parent, nivelActual + 1, maxNiveles, new HashSet<long>(visited));
    }

    private static GenealogyVacuno ToGenealogyVacuno(vacuno entity)
    {
        return new GenealogyVacuno(
            entity.id,
            FormatCodigo(entity.codigo),
            entity.nombre,
            entity.raza_codeNavigation?.nombre ?? entity.raza_code,
            BuildProcedencia(
                entity.granja?.nombre,
                entity.granja?.distrito_codigoNavigation?.nombre,
                entity.granja?.distrito_codigoNavigation?.provincia_codigoNavigation?.nombre,
                entity.granja?.distrito_codigoNavigation?.provincia_codigoNavigation?.departamento_codigoNavigation?.nombre),
            entity.sexo_codeNavigation?.nombre ?? entity.sexo_code,
            entity.padre_id,
            entity.madre_id);
    }

    private static string BuildProcedencia(params string?[] values)
    {
        return string.Join(", ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
    }

    private sealed record GenealogyVacuno(
        long Id,
        string Codigo,
        string Nombre,
        string Raza,
        string Procedencia,
        string Sexo,
        long? PadreId,
        long? MadreId);
}
