using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Common.Gateway.Repositories;
using ZooTech.Application.Modules.Module_Vacunos.UseCases.ListarVacunos;
using ZooTech.Domain.Entities;
using ZooTech.Domain.Enums;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Mappers;
using ZooTech.Infrastructure.Persistence.Models;

namespace ZooTech.Infrastructure.Persistence.Repositories;

public class VacunoRepository : IVacunoRepository
{
    private readonly GanaderiaDbContext _db;

    public VacunoRepository(GanaderiaDbContext db)
    {
        _db = db;
    }

    public async Task<(List<VacunoResumen> Data, int Total)> GetPagedAsync(
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        EstadoAnimal? estado,
        string? q,
        int skip,
        int take)
    {
        // Query base SIN includes
        var query = _db.vacunos
            .AsNoTracking()
            .Where(v => v.deleted_at == null)
            .AsQueryable();

        // Filtro por fecha de registro
        if (fechaDesde.HasValue)
        {
            var desde = DateOnly.FromDateTime(fechaDesde.Value);
            query = query.Where(v => v.fecha_registro >= desde);
        }

        if (fechaHasta.HasValue)
        {
            var hasta = DateOnly.FromDateTime(fechaHasta.Value);
            query = query.Where(v => v.fecha_registro <= hasta);
        }

        // Filtro por estado usando la vista
        if (estado.HasValue)
        {
            var estadoStr = estado.Value.ToString();
            query = query.Where(v => _db.v_vacuno_estado_vigentes
                .Any(e => e.vacuno_id == v.id && e.estado_code == estadoStr));
        }

        // Filtro por búsqueda de texto
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(v => v.codigo.Contains(q) || v.nombre.Contains(q));
        }

        // Total para paginación (rápido sin includes)
        var total = await query.CountAsync();

        // Proyección directa para evitar over-fetching y resolver el estado en el mismo query
        var dataRaw = await query
            .OrderByDescending(v => v.fecha_registro)
            .Skip(skip)
            .Take(take)
            .Select(v => new
            {
                Id = v.id,
                Codigo = v.codigo,
                FechaRegistro = v.fecha_registro,
                Nombre = v.nombre,
                Raza = v.raza_codeNavigation != null ? v.raza_codeNavigation.nombre : v.raza_code,
                Procedencia = v.granja != null 
                    ? v.granja.nombre + " - " + v.granja.distrito_codigoNavigation.nombre + " - " + v.granja.distrito_codigoNavigation.provincia_codigoNavigation.nombre + " - " + v.granja.distrito_codigoNavigation.provincia_codigoNavigation.departamento_codigoNavigation.nombre
                    : "Sin granja",
                EstadoString = _db.v_vacuno_estado_vigentes
                                .Where(e => e.vacuno_id == v.id)
                                .Select(e => e.estado_code)
                                .FirstOrDefault() ?? "VIVO"
            })
            .ToListAsync();

        // Mapeo final en memoria al DTO de Application
        var data = dataRaw.Select(x =>
        {
            var estadoParsed = Enum.TryParse<EstadoAnimal>(x.EstadoString, true, out var e) ? e : EstadoAnimal.VIVO;
            return new VacunoResumen
            {
                Id = x.Id,
                Codigo = x.Codigo,
                FechaRegistro = x.FechaRegistro.ToDateTime(TimeOnly.MinValue),
                Nombre = x.Nombre,
                Raza = x.Raza,
                Procedencia = x.Procedencia,
                Estado = estadoParsed
            };
        }).ToList();

        return (data, total);
    }

    public async Task<ZooTech.Application.Modules.Module_Vacunos.UseCases.GenerarArbolGenealogico.VacunoNodoDto?> GetArbolGenealogicoAsync(long vacunoId, int niveles)
    {
        if (_db.Database.IsSqlServer())
        {
            return await GetArbolSqlAsync(vacunoId, niveles);
        }
        else
        {
            return await GetArbolInMemoryAsync(vacunoId, niveles);
        }
    }

    private async Task<ZooTech.Application.Modules.Module_Vacunos.UseCases.GenerarArbolGenealogico.VacunoNodoDto?> GetArbolSqlAsync(long vacunoId, int niveles)
    {
        // CTE Recursiva que extrae todos los ancestros hasta el nivel solicitado.
        // El LEFT JOIN se hace fuera de la CTE recursiva por limitaciones de SQL Server.
        var sql = @"
            WITH Ancestros AS (
                -- Nivel 0: El vacuno raíz
                SELECT 
                    v.id, v.codigo, v.nombre, v.raza_code, v.sexo_code,
                    v.padre_id, v.madre_id, 0 AS Nivel
                FROM vacuno v
                WHERE v.id = {0} AND v.deleted_at IS NULL

                UNION ALL

                -- Niveles > 0: Padres y Madres
                SELECT 
                    p.id, p.codigo, p.nombre, p.raza_code, p.sexo_code,
                    p.padre_id, p.madre_id, a.Nivel + 1
                FROM Ancestros a
                JOIN vacuno p ON (a.padre_id = p.id OR a.madre_id = p.id)
                WHERE p.deleted_at IS NULL AND a.Nivel < {1}
            )
            SELECT DISTINCT 
                a.id as Id, 
                a.codigo as Codigo, 
                a.nombre as Nombre, 
                ISNULL(r.nombre, a.raza_code) as Raza, 
                a.sexo_code as Sexo, 
                a.padre_id as PadreId, 
                a.madre_id as MadreId, 
                a.Nivel as Nivel
            FROM Ancestros a
            LEFT JOIN cat_raza r ON a.raza_code = r.code;
        ";

        var ancestrosPlano = await _db.Database.SqlQueryRaw<AncestroDbDto>(sql, vacunoId, niveles).ToListAsync();
        var arbolDb = ConstruirArbol(ancestrosPlano, vacunoId, 0, niveles);
        return MapearADtoApplication(arbolDb);
    }

    private async Task<ZooTech.Application.Modules.Module_Vacunos.UseCases.GenerarArbolGenealogico.VacunoNodoDto?> GetArbolInMemoryAsync(long vacunoId, int niveles)
    {
        // Fallback ineficiente pero funcional para InMemory Database (útil para Unit Tests locales)
        var vacunosInMemory = await _db.vacunos
            .Include(v => v.raza_codeNavigation)
            .Where(v => v.deleted_at == null)
            .ToListAsync(); // Trae todo a memoria porque no podemos hacer CTE

        var flatList = new List<AncestroDbDto>();
        void RecorrerMemoria(long id, int nivelActual)
        {
            if (nivelActual > niveles) return;
            var v = vacunosInMemory.FirstOrDefault(x => x.id == id);
            if (v == null || flatList.Any(f => f.Id == id)) return; // Evita ciclos

            flatList.Add(new AncestroDbDto
            {
                Id = v.id,
                Codigo = v.codigo,
                Nombre = v.nombre,
                Raza = v.raza_codeNavigation?.nombre ?? v.raza_code,
                Sexo = v.sexo_code,
                PadreId = v.padre_id,
                MadreId = v.madre_id,
                Nivel = nivelActual
            });

            if (v.padre_id.HasValue) RecorrerMemoria(v.padre_id.Value, nivelActual + 1);
            if (v.madre_id.HasValue) RecorrerMemoria(v.madre_id.Value, nivelActual + 1);
        }

        RecorrerMemoria(vacunoId, 0);
        var arbolDb = ConstruirArbol(flatList, vacunoId, 0, niveles);
        return MapearADtoApplication(arbolDb);
    }

    private AncestroDbDto? ConstruirArbol(List<AncestroDbDto> planos, long actualId, int nivelActual, int maxNiveles)
    {
        if (nivelActual > maxNiveles) return null;

        var dict = planos.ToDictionary(p => p.Id);
        return ConstruirNodo(dict, actualId, nivelActual, maxNiveles);
    }

    private AncestroDbDto? ConstruirNodo(Dictionary<long, AncestroDbDto> dict, long actualId, int nivelActual, int maxNiveles)
    {
        if (nivelActual > maxNiveles) return null;

        if (!dict.TryGetValue(actualId, out var nodoDb)) return null;

        // Evitar re-ensamblar si ya fue procesado o romper ciclos
        var nodoCopia = new AncestroDbDto
        {
            Id = nodoDb.Id,
            Codigo = nodoDb.Codigo,
            Nombre = nodoDb.Nombre,
            Raza = nodoDb.Raza,
            Sexo = nodoDb.Sexo,
            Nivel = nivelActual
        };

        if (nodoDb.PadreId.HasValue)
            nodoCopia.Padre = ConstruirNodo(dict, nodoDb.PadreId.Value, nivelActual + 1, maxNiveles);

        if (nodoDb.MadreId.HasValue)
            nodoCopia.Madre = ConstruirNodo(dict, nodoDb.MadreId.Value, nivelActual + 1, maxNiveles);

        return nodoCopia;
    }

    private ZooTech.Application.Modules.Module_Vacunos.UseCases.GenerarArbolGenealogico.VacunoNodoDto? MapearADtoApplication(AncestroDbDto? dbDto)
    {
        if (dbDto == null) return null;

        return new ZooTech.Application.Modules.Module_Vacunos.UseCases.GenerarArbolGenealogico.VacunoNodoDto
        {
            Id = dbDto.Id,
            Codigo = dbDto.Codigo,
            Nombre = dbDto.Nombre,
            Raza = dbDto.Raza,
            Sexo = dbDto.Sexo,
            Nivel = dbDto.Nivel,
            Padre = MapearADtoApplication(dbDto.Padre),
            Madre = MapearADtoApplication(dbDto.Madre)
        };
    }
}
