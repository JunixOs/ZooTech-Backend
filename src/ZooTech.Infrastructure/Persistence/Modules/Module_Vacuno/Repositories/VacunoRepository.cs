using Microsoft.EntityFrameworkCore;
using ZooTech.Domain.Module_Vacuno.Entities;
using ZooTech.Domain.Module_Vacuno.Interfaces;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Application.Modules.Module_Vacuno.Common;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GenerarArbolGenealogico;
using ZooTech.Infrastructure.Persistence.Models;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_Vacuno.Repositories;

public sealed class VacunoRepository : IVacunoRepository, IVacunoQueryRepository
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

    public async Task<List<Vacuno>> ListAllWithDeletedAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.vacunos
            .AsNoTracking()
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

    public async Task<Vacuno?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken = default)
    {
        var entity = await _context.vacunos
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.codigo == codigo.Trim() && v.deleted_at == null, cancellationToken);

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

    public async Task<(List<(Vacuno Vacuno, string? Procedencia)> Items, int TotalCount)> GetPagedAsync(
        string? query, DateTime? fechaDesde, DateTime? fechaHasta, int page, int limit, CancellationToken cancellationToken = default)
    {
        var q = _context.vacunos
            .AsNoTracking()
            .Where(v => v.deleted_at == null);

        if (!string.IsNullOrWhiteSpace(query))
        {
            var lowerQuery = query.ToLower();
            q = q.Where(v => v.codigo.ToLower().Contains(lowerQuery) || v.nombre.ToLower().Contains(lowerQuery));
        }

        if (fechaDesde.HasValue)
        {
            var fd = DateOnly.FromDateTime(fechaDesde.Value);
            q = q.Where(v => v.fecha_registro >= fd);
        }

        if (fechaHasta.HasValue)
        {
            var fh = DateOnly.FromDateTime(fechaHasta.Value);
            q = q.Where(v => v.fecha_registro <= fh);
        }

        var totalCount = await q.CountAsync(cancellationToken);

        var entities = await q
            .OrderByDescending(v => v.fecha_registro)
            .ThenByDescending(v => v.id)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Include(v => v.granja)
                .ThenInclude(g => g.distrito_codigoNavigation)
                    .ThenInclude(d => d.provincia_codigoNavigation)
                        .ThenInclude(p => p.departamento_codigoNavigation)
            .ToListAsync(cancellationToken);

        var items = entities.Select(v =>
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

        return (items, totalCount);
    }

    public async Task<List<Vacuno>> GetArbolGenealogicoAsync(long id, int maxNiveles, CancellationToken cancellationToken = default)
    {
        var query = $@"
            WITH CTE AS (
                SELECT *, 1 AS Nivel
                FROM vacuno
                WHERE id = {{0}} AND deleted_at IS NULL
                
                UNION ALL
                
                SELECT v.*, CTE.Nivel + 1
                FROM vacuno v
                INNER JOIN CTE ON (v.id = CTE.padre_id OR v.id = CTE.madre_id)
                WHERE CTE.Nivel < {{1}} AND v.deleted_at IS NULL
            )
            SELECT DISTINCT * FROM CTE ORDER BY Nivel, id;
        ";

        var entities = await _context.vacunos
            .FromSqlRaw(query, id, maxNiveles)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain).ToList();
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
            numChip: null,
            fechaRegistro: entity.fecha_registro,
            createdAt: entity.created_at,
            updatedAt: entity.updated_at,
            deletedAt: entity.deleted_at,
            motivoEliminacion: entity.motivo_eliminacion,
            createdBy: entity.created_by,
            updatedBy: entity.updated_by,
            deletedBy: entity.deleted_by);
    }

    public async Task<VacunoNodoDto?> GetArbolGenealogicoAsync(long vacunoId, int niveles)
    {
        if (_context.Database.IsSqlServer())
        {
            return await GetArbolSqlAsync(vacunoId, niveles);
        }
        else
        {
            return await GetArbolInMemoryAsync(vacunoId, niveles);
        }
    }

    private async Task<VacunoNodoDto?> GetArbolSqlAsync(long vacunoId, int niveles)
    {
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

        var ancestrosPlano = await _context.Database.SqlQueryRaw<AncestroDbDto>(sql, vacunoId, niveles).ToListAsync();
        var arbolDb = ConstruirArbol(ancestrosPlano, vacunoId, 0, niveles);
        return MapearADtoApplication(arbolDb);
    }

    private async Task<VacunoNodoDto?> GetArbolInMemoryAsync(long vacunoId, int niveles)
    {
        var vacunosInMemory = await _context.vacunos
            .Include(v => v.raza_codeNavigation)
            .Where(v => v.deleted_at == null)
            .ToListAsync();

        var flatList = new List<AncestroDbDto>();
        void RecorrerMemoria(long id, int nivelActual)
        {
            if (nivelActual > niveles) return;
            var v = vacunosInMemory.FirstOrDefault(x => x.id == id);
            if (v == null || flatList.Any(f => f.Id == id)) return;

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

    private VacunoNodoDto? MapearADtoApplication(AncestroDbDto? dbDto)
    {
        if (dbDto == null) return null;

        return new VacunoNodoDto
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
