using Microsoft.EntityFrameworkCore;
using ZooTech.Domain.Module_Celo.Entities;
using ZooTech.Domain.Module_Celo.Interfaces;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_Celo.Repositories;

public sealed class CeloRepository : ICeloRepository
{
    private readonly GanaderiaDbContext _ganaderiaDbContext;

    public CeloRepository(
        IGanaderiaDbContextFactory ganaderiaDbContextFactory
    )
    {
        _ganaderiaDbContext  = ganaderiaDbContextFactory.CreateDbContextByTenantContext();
    }

    public async Task<List<CeloListItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _ganaderiaDbContext.celo_registros
            .AsNoTracking()
            .Where(c => c.deleted_at == null)
            .Select(c => new celo_registro
            {
                id = c.id,
                codigo = c.codigo,
                fecha_hora = c.fecha_hora,
                vacuno_id = c.vacuno_id,

                vacuno = new vacuno
                {
                    id = c.vacuno.id,
                    codigo = c.vacuno.codigo,
                    nombre = c.vacuno.nombre,
                    raza_code = c.vacuno.raza_code,

                },

            })
            
            .OrderByDescending(c => c.fecha_hora)
            .ToListAsync(cancellationToken);

        return entities.Select(ListAllCeloToDomain).ToList();
    }

    public async Task<List<CeloReporteItem>> GetAllForReporteAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _ganaderiaDbContext.celo_registros
            .AsNoTracking()
            .Include(c => c.caracteristica_codes)
            .Where(c => c.deleted_at == null)
            .Select(c => new celo_registro
            {
                id = c.id,
                codigo = c.codigo,
                fecha_hora = c.fecha_hora,
                vacuno_id = c.vacuno_id,
                observaciones = c.observaciones,
                caracteristica_codes = c.caracteristica_codes,

                vacuno = new vacuno
                {
                    id = c.vacuno.id,
                    codigo = c.vacuno.codigo,
                    nombre = c.vacuno.nombre,
                },
            })
            .OrderByDescending(c => c.fecha_hora)
            .ToListAsync(cancellationToken);

        return entities.Select(ReporteCeloToDomain).ToList();
    }

    public async Task<(IReadOnlyList<CeloListItem> Items, int TotalCount)> GetPagedAsync(
        string? search,
        int page,
        int pageSize,
        DateTime? fechaInicio = null,
        DateTime? fechaFin = null,
        IReadOnlyDictionary<string, string>? columnFilters = null,
        CancellationToken cancellationToken = default)
    {
        var queryable = _ganaderiaDbContext.celo_registros
            .AsNoTracking()
            .Where(c => c.deleted_at == null);

        if (!string.IsNullOrWhiteSpace(search))
        {
            queryable = queryable.Where(c =>
                c.codigo.Contains(search) ||
                c.vacuno.codigo.Contains(search) ||
                c.vacuno.nombre.Contains(search));
        }

        queryable = ApplyFechaRangeFilter(queryable, fechaInicio, fechaFin);

        // Tier 1: direct SQL-translatable column filters (row-level columns).
        if (columnFilters is not null)
        {
            queryable = ApplyCommonColumnFilters(queryable, columnFilters);

            // Tier 2: "vecesEnCelo" is not a row column — it's a historical count per
            // vacuno computed over the whole table (GetVecesEnCeloCountsAsync) and
            // attached after pagination by the interactor. To filter by it we first
            // resolve which VacunoId's count text matches the search value, then
            // narrow the queryable with `vacuno_id IN (...)` before paginating.
            if (columnFilters.TryGetValue("vecesEnCelo", out var vecesEnCeloFilter) &&
                !string.IsNullOrWhiteSpace(vecesEnCeloFilter))
            {
                queryable = await ApplyVecesEnCeloFilterAsync(queryable, vecesEnCeloFilter, cancellationToken);
            }
        }

        // Tier 3: "fecha"/"hora" match on the formatted text of `fecha_hora`.
        // DateTime.ToString() isn't reliably translatable to SQL across EF Core/
        // provider versions, so once either key is present we bring the already
        // narrowed candidate set (search + date range + tiers 1-2 above) into
        // memory, format + filter it there, and paginate in-memory — instead of
        // risking a fragile SQL translation. This path is only taken when the
        // fecha/hora column filter is actually used; the default path below stays
        // 100% SQL with Skip/Take.
        var fechaFilterProvided = columnFilters is not null &&
            columnFilters.TryGetValue("fecha", out var fechaFilterRaw) &&
            !string.IsNullOrWhiteSpace(fechaFilterRaw);
        var horaFilterProvided = columnFilters is not null &&
            columnFilters.TryGetValue("hora", out var horaFilterRaw) &&
            !string.IsNullOrWhiteSpace(horaFilterRaw);

        if (fechaFilterProvided || horaFilterProvided)
        {
            var fechaFilter = fechaFilterProvided ? columnFilters!["fecha"] : null;
            var horaFilter = horaFilterProvided ? columnFilters!["hora"] : null;

            var candidates = await queryable
                .Select(c => new celo_registro
                {
                    id = c.id,
                    codigo = c.codigo,
                    fecha_hora = c.fecha_hora,
                    vacuno_id = c.vacuno_id,

                    vacuno = new vacuno
                    {
                        id = c.vacuno.id,
                        codigo = c.vacuno.codigo,
                        nombre = c.vacuno.nombre,
                        raza_code = c.vacuno.raza_code,
                    },
                })
                .OrderByDescending(c => c.fecha_hora)
                .ToListAsync(cancellationToken);

            var filteredCandidates = candidates
                .Where(c =>
                    (string.IsNullOrWhiteSpace(fechaFilter) || c.fecha_hora.ToString("yyyy-MM-dd").Contains(fechaFilter)) &&
                    (string.IsNullOrWhiteSpace(horaFilter) || c.fecha_hora.ToString("HH:mm:ss").Contains(horaFilter)))
                .ToList();

            var totalCountInMemory = filteredCandidates.Count;
            var pagedInMemory = filteredCandidates
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (pagedInMemory.Select(ListAllCeloToDomain).ToList(), totalCountInMemory);
        }

        var totalCount = await queryable.CountAsync(cancellationToken);

        var entities = await queryable
            .Select(c => new celo_registro
            {
                id = c.id,
                codigo = c.codigo,
                fecha_hora = c.fecha_hora,
                vacuno_id = c.vacuno_id,

                vacuno = new vacuno
                {
                    id = c.vacuno.id,
                    codigo = c.vacuno.codigo,
                    nombre = c.vacuno.nombre,
                    raza_code = c.vacuno.raza_code,
                },
            })
            .OrderByDescending(c => c.fecha_hora)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = entities.Select(ListAllCeloToDomain).ToList();
        return (items, totalCount);
    }

    public async Task<(IReadOnlyList<CeloReporteItem> Items, int TotalCount)> GetPagedForReporteAsync(
        string? search,
        int page,
        int pageSize,
        DateTime? fechaInicio = null,
        DateTime? fechaFin = null,
        IReadOnlyDictionary<string, string>? columnFilters = null,
        CancellationToken cancellationToken = default)
    {
        var queryable = _ganaderiaDbContext.celo_registros
            .AsNoTracking()
            .Where(c => c.deleted_at == null);

        if (!string.IsNullOrWhiteSpace(search))
        {
            queryable = queryable.Where(c =>
                c.codigo.Contains(search) ||
                c.vacuno.codigo.Contains(search) ||
                c.vacuno.nombre.Contains(search) ||
                (c.observaciones != null && c.observaciones.Contains(search)));
        }

        queryable = ApplyFechaRangeFilter(queryable, fechaInicio, fechaFin);

        // Tier 1: direct SQL-translatable column filters (row-level columns +
        // the `caracteristica_codes` navigation, which EF Core translates fine
        // via `.Any(...)` without needing an explicit `Include` for filtering).
        if (columnFilters is not null)
        {
            queryable = ApplyCommonColumnFilters(queryable, columnFilters);

            if (columnFilters.TryGetValue("caracteristicas", out var caracteristicasFilter) &&
                !string.IsNullOrWhiteSpace(caracteristicasFilter))
            {
                queryable = queryable.Where(c => c.caracteristica_codes.Any(cc =>
                    cc.code.Contains(caracteristicasFilter) || cc.nombre.Contains(caracteristicasFilter)));
            }

            // Tier 2: "vecesEnCelo"/"crias" are not row columns — they're historical
            // counts per vacuno computed over the whole table
            // (GetVecesEnCeloCountsAsync/GetCriasCountsAsync) and attached after
            // pagination by the interactor. To filter by either, we first resolve
            // which VacunoId's count text matches the search value, then narrow the
            // queryable with `vacuno_id IN (...)` before paginating. Each counts
            // query only runs when its matching filter key is present, to avoid an
            // unconditional extra round-trip.
            if (columnFilters.TryGetValue("vecesEnCelo", out var vecesEnCeloFilter) &&
                !string.IsNullOrWhiteSpace(vecesEnCeloFilter))
            {
                queryable = await ApplyVecesEnCeloFilterAsync(queryable, vecesEnCeloFilter, cancellationToken);
            }

            if (columnFilters.TryGetValue("crias", out var criasFilter) &&
                !string.IsNullOrWhiteSpace(criasFilter))
            {
                var criasCounts = await GetCriasCountsAsync(cancellationToken);
                var matchingVacunoIds = criasCounts
                    .Where(kvp => kvp.Value.ToString().Contains(criasFilter))
                    .Select(kvp => kvp.Key)
                    .ToHashSet();

                queryable = queryable.Where(c => matchingVacunoIds.Contains(c.vacuno_id));
            }
        }

        // Tier 3: "fecha"/"hora" match on the formatted text of `fecha_hora`.
        // DateTime.ToString() isn't reliably translatable to SQL across EF Core/
        // provider versions, so once either key is present we bring the already
        // narrowed candidate set (search + date range + tiers 1-2 above) into
        // memory, format + filter it there, and paginate in-memory — instead of
        // risking a fragile SQL translation. This path is only taken when the
        // fecha/hora column filter is actually used; the default path below stays
        // 100% SQL with Skip/Take.
        var fechaFilterProvided = columnFilters is not null &&
            columnFilters.TryGetValue("fecha", out var fechaFilterRaw) &&
            !string.IsNullOrWhiteSpace(fechaFilterRaw);
        var horaFilterProvided = columnFilters is not null &&
            columnFilters.TryGetValue("hora", out var horaFilterRaw) &&
            !string.IsNullOrWhiteSpace(horaFilterRaw);

        if (fechaFilterProvided || horaFilterProvided)
        {
            var fechaFilter = fechaFilterProvided ? columnFilters!["fecha"] : null;
            var horaFilter = horaFilterProvided ? columnFilters!["hora"] : null;

            var candidates = await queryable
                .Include(c => c.caracteristica_codes)
                .Select(c => new celo_registro
                {
                    id = c.id,
                    codigo = c.codigo,
                    fecha_hora = c.fecha_hora,
                    vacuno_id = c.vacuno_id,
                    observaciones = c.observaciones,
                    caracteristica_codes = c.caracteristica_codes,

                    vacuno = new vacuno
                    {
                        id = c.vacuno.id,
                        codigo = c.vacuno.codigo,
                        nombre = c.vacuno.nombre,
                    },
                })
                .OrderByDescending(c => c.fecha_hora)
                .ToListAsync(cancellationToken);

            var filteredCandidates = candidates
                .Where(c =>
                    (string.IsNullOrWhiteSpace(fechaFilter) || c.fecha_hora.ToString("yyyy-MM-dd").Contains(fechaFilter)) &&
                    (string.IsNullOrWhiteSpace(horaFilter) || c.fecha_hora.ToString("HH:mm:ss").Contains(horaFilter)))
                .ToList();

            var totalCountInMemory = filteredCandidates.Count;
            var pagedInMemory = filteredCandidates
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (pagedInMemory.Select(ReporteCeloToDomain).ToList(), totalCountInMemory);
        }

        var totalCount = await queryable.CountAsync(cancellationToken);

        var entities = await queryable
            .Include(c => c.caracteristica_codes)
            .Select(c => new celo_registro
            {
                id = c.id,
                codigo = c.codigo,
                fecha_hora = c.fecha_hora,
                vacuno_id = c.vacuno_id,
                observaciones = c.observaciones,
                caracteristica_codes = c.caracteristica_codes,

                vacuno = new vacuno
                {
                    id = c.vacuno.id,
                    codigo = c.vacuno.codigo,
                    nombre = c.vacuno.nombre,
                },
            })
            .OrderByDescending(c => c.fecha_hora)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = entities.Select(ReporteCeloToDomain).ToList();
        return (items, totalCount);
    }

    public async Task<Dictionary<long, int>> GetVecesEnCeloCountsAsync(CancellationToken cancellationToken = default)
    {
        return await _ganaderiaDbContext.celo_registros
            .AsNoTracking()
            .Where(c => c.deleted_at == null)
            .GroupBy(c => c.vacuno_id)
            .Select(g => new { VacunoId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.VacunoId, x => x.Count, cancellationToken);
    }

    public async Task<Dictionary<long, int>> GetCriasCountsAsync(CancellationToken cancellationToken = default)
    {
        return await _ganaderiaDbContext.vacunos
            .AsNoTracking()
            .Where(v => v.deleted_at == null && v.madre_id != null)
            .GroupBy(v => v.madre_id!.Value)
            .Select(g => new { MadreId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.MadreId, x => x.Count, cancellationToken);
    }

    public async Task<Celo?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _ganaderiaDbContext.celo_registros
            .AsNoTracking()
            .Include(c => c.caracteristica_codes)
            .Include(c => c.vacuno)
            .FirstOrDefaultAsync(c => c.id == id && c.deleted_at == null, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<Celo> AddAsync(Celo celo, CancellationToken cancellationToken = default)
    {
        var entity = ToEntity(celo);

        if (celo.CaracteristicaCodes.Count > 0)
        {
            var caracteristicas = await _ganaderiaDbContext.cat_caracteristica_celos
                .Where(c => celo.CaracteristicaCodes.Contains(c.code))
                .ToListAsync(cancellationToken);

            foreach (var caracteristica in caracteristicas)
            {
                entity.caracteristica_codes.Add(caracteristica);
            }
        }

        await _ganaderiaDbContext.celo_registros.AddAsync(entity, cancellationToken);
        await _ganaderiaDbContext.SaveChangesAsync(cancellationToken);

        await _ganaderiaDbContext.Entry(entity)
            .Reference(e => e.vacuno)
            .LoadAsync(cancellationToken);

        await _ganaderiaDbContext.Entry(entity)
            .Collection(e => e.caracteristica_codes)
            .LoadAsync(cancellationToken);

        return ToDomain(entity);
    }

    public async Task<Celo> UpdateAsync(Celo celo, CancellationToken cancellationToken = default)
    {
        var entity = await _ganaderiaDbContext.celo_registros
            .Include(c => c.caracteristica_codes)
            .FirstOrDefaultAsync(c => c.id == celo.Id && c.deleted_at == null, cancellationToken);

        if (entity is null)
            throw new InvalidOperationException($"No se encontró el registro de celo con ID {celo.Id}.");

        entity.observaciones = celo.Observaciones;
        entity.updated_at = celo.UpdatedAt;
        entity.updated_by = celo.UpdatedBy;

        entity.caracteristica_codes.Clear();
        if (celo.CaracteristicaCodes.Count > 0)
        {
            var caracteristicas = await _ganaderiaDbContext.cat_caracteristica_celos
                .Where(c => celo.CaracteristicaCodes.Contains(c.code))
                .ToListAsync(cancellationToken);

            foreach (var caracteristica in caracteristicas)
                entity.caracteristica_codes.Add(caracteristica);
        }

        entity.deleted_at = celo.DeletedAt;
        entity.deleted_by = celo.DeletedBy;
        entity.motivo_eliminacion = celo.MotivoEliminacion;

        await _ganaderiaDbContext.SaveChangesAsync(cancellationToken);

        return ToDomain(entity);
    }

    public async Task<bool> ExistsVacunoAsync(long vacunoId, CancellationToken cancellationToken = default)
    {
        return await _ganaderiaDbContext.vacunos
            .AnyAsync(v => v.id == vacunoId, cancellationToken);
    }

    public async Task<bool> ExistsCodigoAsync(string codigo, CancellationToken cancellationToken = default)
    {
        return await _ganaderiaDbContext.celo_registros
            .AnyAsync(c => c.codigo == codigo && c.deleted_at == null, cancellationToken);
    }

    public async Task<List<DateTime>> GetByDateRangeAsync(
        DateTime? fechaInicio,
        DateTime? fechaFin,
        CancellationToken cancellationToken = default)
    {
        var query = _ganaderiaDbContext.celo_registros
            .AsNoTracking()
            .Where(c => c.deleted_at == null);

        query = ApplyFechaRangeFilter(query, fechaInicio, fechaFin);

        return await query
            .OrderBy(c => c.fecha_hora)
            .Select(c => c.fecha_hora)
            .ToListAsync(cancellationToken);
    }

    private static IQueryable<Entities.celo_registro> ApplyFechaRangeFilter(
        IQueryable<Entities.celo_registro> queryable,
        DateTime? fechaInicio,
        DateTime? fechaFin)
    {
        if (fechaInicio.HasValue)
        {
            queryable = queryable.Where(c => c.fecha_hora >= fechaInicio.Value);
        }

        if (fechaFin.HasValue)
        {
            var fechaFinInclusive = fechaFin.Value.Date.AddDays(1).AddTicks(-1);
            queryable = queryable.Where(c => c.fecha_hora <= fechaFinInclusive);
        }

        return queryable;
    }

    private static IQueryable<Entities.celo_registro> ApplyCommonColumnFilters(
        IQueryable<Entities.celo_registro> queryable,
        IReadOnlyDictionary<string, string> columnFilters)
    {
        if (columnFilters.TryGetValue("codigoRegistro", out var codigoRegistroFilter) &&
            !string.IsNullOrWhiteSpace(codigoRegistroFilter))
        {
            queryable = queryable.Where(c => c.codigo.Contains(codigoRegistroFilter));
        }

        if (columnFilters.TryGetValue("codigoVacuno", out var codigoVacunoFilter) &&
            !string.IsNullOrWhiteSpace(codigoVacunoFilter))
        {
            queryable = queryable.Where(c => c.vacuno.codigo.Contains(codigoVacunoFilter));
        }

        if (columnFilters.TryGetValue("nombreVacuno", out var nombreVacunoFilter) &&
            !string.IsNullOrWhiteSpace(nombreVacunoFilter))
        {
            queryable = queryable.Where(c => c.vacuno.nombre.Contains(nombreVacunoFilter));
        }

        return queryable;
    }

    private async Task<IQueryable<Entities.celo_registro>> ApplyVecesEnCeloFilterAsync(
        IQueryable<Entities.celo_registro> queryable,
        string vecesEnCeloFilter,
        CancellationToken cancellationToken)
    {
        var counts = await GetVecesEnCeloCountsAsync(cancellationToken);
        var matchingVacunoIds = counts
            .Where(kvp => kvp.Value.ToString().Contains(vecesEnCeloFilter))
            .Select(kvp => kvp.Key)
            .ToHashSet();

        return queryable.Where(c => matchingVacunoIds.Contains(c.vacuno_id));
    }

    private static Celo ToDomain(Entities.celo_registro entity)
    {
        return Celo.Rehydrate(
            id: entity.id,
            codigo: entity.codigo,
            fechaHora: entity.fecha_hora,
            vacunoId: entity.vacuno_id,
            vacunoCodigo: entity.vacuno?.codigo ?? string.Empty,
            nombreVacuno: entity.vacuno?.nombre ?? string.Empty,
            encargadoUsuarioId: entity.encargado_usuario_id,
            observaciones: entity.observaciones,
            estadoRegistroCode: entity.estado_registro_code,
            caracteristicaCodes: entity.caracteristica_codes
                .Select(c => c.code)
                .ToList(),
            createdAt: entity.created_at,
            updatedAt: entity.updated_at,
            deletedAt: entity.deleted_at,
            motivoEliminacion: entity.motivo_eliminacion,
            createdBy: entity.created_by,
            updatedBy: entity.updated_by,
            deletedBy: entity.deleted_by);
    }


    private static CeloListItem ListAllCeloToDomain(Entities.celo_registro entity)

    {
        return CeloListItem.Rehydrate(
            id: entity.id,
            codigo: entity.codigo,
            fechaHora: entity.fecha_hora,
            vacunoId: entity.vacuno_id,
            vacunoCodigo: entity.vacuno?.codigo ?? string.Empty,
            nombreVacuno: entity.vacuno?.nombre ?? string.Empty);


    } 

    private static CeloReporteItem ReporteCeloToDomain(Entities.celo_registro entity)
    {
        return CeloReporteItem.Rehydrate(
            id: entity.id,
            codigo: entity.codigo,
            fechaHora: entity.fecha_hora,
            vacunoId: entity.vacuno_id,
            vacunoCodigo: entity.vacuno?.codigo ?? string.Empty,
            nombreVacuno: entity.vacuno?.nombre ?? string.Empty,
            observaciones: entity.observaciones ?? string.Empty,
            caracteristicaCodes: entity.caracteristica_codes
                .Select(c => c.code)
                .ToList());
    }

    private static Entities.celo_registro ToEntity(Celo celo)
    {
        return new Entities.celo_registro
        {
            id = celo.Id,
            codigo = celo.Codigo,
            fecha_hora = celo.FechaHora,
            vacuno_id = celo.VacunoId,
            encargado_usuario_id = celo.EncargadoUsuarioId,
            observaciones = celo.Observaciones,
            estado_registro_code = celo.EstadoRegistroCode,
            created_at = celo.CreatedAt,
            updated_at = celo.UpdatedAt,
            deleted_at = celo.DeletedAt,
            motivo_eliminacion = celo.MotivoEliminacion,
            created_by = celo.CreatedBy,
            updated_by = celo.UpdatedBy,
            deleted_by = celo.DeletedBy
        };
    }
}
