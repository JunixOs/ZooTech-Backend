using Microsoft.EntityFrameworkCore;
using ZooTech.Domain.Module_Vacuno.Entities;
using ZooTech.Domain.Module_Vacuno.Interfaces;
using ZooTech.Domain.Module_Vacuno.Entities.GetArbolGenealogico;
using ZooTech.Domain.Module_Vacuno.Entities.ListarVacuno;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Domain.Module_Vacuno.Models;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_Vacuno.Repositories;

public sealed class VacunoRepository : IVacunoRepository
{
    private readonly GanaderiaDbContext _ganaderiaDbContext;

    public VacunoRepository(
        IGanaderiaDbContextFactory ganaderiaDbContextFactory
    )
    {
        _ganaderiaDbContext = ganaderiaDbContextFactory.CreateDbContextByTenantContext();
    }

    public async Task<List<Vacuno>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _ganaderiaDbContext.vacunos
            .AsNoTracking()
            .Where(v => v.deleted_at == null)
            .OrderBy(v => v.codigo)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain).ToList();

    }

    public async Task<List<Vacuno>> ListAllWithDeletedAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _ganaderiaDbContext.vacunos
            .AsNoTracking()
            .OrderBy(v => v.codigo)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain).ToList();
    }

    public async Task<List<(Vacuno Vacuno, string? Procedencia)>> ListAllForDisplayAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _ganaderiaDbContext.vacunos
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
        var entity = await _ganaderiaDbContext.vacunos
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.id == id && v.deleted_at == null, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<Vacuno?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken = default)
    {
        var entity = await _ganaderiaDbContext.vacunos
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.codigo == codigo.Trim() && v.deleted_at == null, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default) => 
            await _ganaderiaDbContext.vacunos
                .AnyAsync(v => v.id == id && v.deleted_at == null, cancellationToken);

    public Task<bool> ExistsCodigoAsync(string codigo, CancellationToken cancellationToken = default)
        => _ganaderiaDbContext.vacunos.AnyAsync(v => v.codigo == codigo.Trim(), cancellationToken);

    public async Task<Vacuno> AddAsync(Vacuno vacuno, decimal? precioCompra, string? aptoPara, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _ganaderiaDbContext.Database.BeginTransactionAsync(cancellationToken);
        var entity = ToEntity(vacuno);
        _ganaderiaDbContext.vacunos.Add(entity);

        var now = DateTime.UtcNow;
        await _ganaderiaDbContext.SaveChangesAsync(cancellationToken);

        if (precioCompra.HasValue)
        {
            var adq = new ZooTech.Infrastructure.Persistence.Entities.vacuno_adquisicion
            {
                vacuno_id = entity.id,
                tipo_adquisicion_code = entity.tipo_adquisicion_code,
                fecha_adquisicion = DateOnly.FromDateTime(now),
                precio_compra = precioCompra.Value,
                created_at = now
            };
            _ganaderiaDbContext.vacuno_adquisicions.Add(adq);
        }

        if (!string.IsNullOrEmpty(aptoPara))
        {
            var util = new ZooTech.Infrastructure.Persistence.Entities.vacuno_utilizacion_historial
            {
                vacuno_id = entity.id,
                tipo_utilizacion_code = aptoPara,
                created_at = now
            };
            _ganaderiaDbContext.vacuno_utilizacion_historials.Add(util);
        }

        var est = new ZooTech.Infrastructure.Persistence.Entities.vacuno_estado_historial
        {
            vacuno_id = entity.id,
            estado_code = "SANO",
            fecha_estado = DateOnly.FromDateTime(now),
            created_at = now
        };
        _ganaderiaDbContext.vacuno_estado_historials.Add(est);

        await _ganaderiaDbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return ToDomain(entity);
    }

    public async Task<Vacuno> UpdateAsync(Vacuno vacuno, decimal? precioCompra, string? aptoPara, CancellationToken cancellationToken = default)
    {
        var entity = await _ganaderiaDbContext.vacunos
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

        var now = DateTime.UtcNow;

        var existingAdq = await _ganaderiaDbContext.vacuno_adquisicions.FirstOrDefaultAsync(a => a.vacuno_id == entity.id, cancellationToken);
        if (existingAdq != null)
        {
            existingAdq.precio_compra = precioCompra;
            existingAdq.tipo_adquisicion_code = entity.tipo_adquisicion_code;
        }
        else if (precioCompra.HasValue)
        {
            var adq = new ZooTech.Infrastructure.Persistence.Entities.vacuno_adquisicion
            {
                vacuno_id = entity.id,
                tipo_adquisicion_code = entity.tipo_adquisicion_code,
                fecha_adquisicion = DateOnly.FromDateTime(now),
                precio_compra = precioCompra.Value,
                created_at = now
            };
            _ganaderiaDbContext.vacuno_adquisicions.Add(adq);
        }

        var currentUtil = await _ganaderiaDbContext.vacuno_utilizacion_historials
            .Where(u => u.vacuno_id == entity.id)
            .OrderByDescending(u => u.created_at)
            .FirstOrDefaultAsync(cancellationToken);

        if (currentUtil == null || currentUtil.tipo_utilizacion_code != aptoPara)
        {
            if (!string.IsNullOrEmpty(aptoPara))
            {
                var util = new ZooTech.Infrastructure.Persistence.Entities.vacuno_utilizacion_historial
                {
                    vacuno_id = entity.id,
                    tipo_utilizacion_code = aptoPara,
                    created_at = now
                };
                _ganaderiaDbContext.vacuno_utilizacion_historials.Add(util);
            }
        }

        if (vacuno.IsDeleted)
        {
            var existingDeletedState = await _ganaderiaDbContext.vacuno_estado_historials
                .AnyAsync(eh => eh.vacuno_id == entity.id && eh.estado_code == "MUERTO", cancellationToken);

            if (!existingDeletedState)
            {
                var est = new ZooTech.Infrastructure.Persistence.Entities.vacuno_estado_historial
                {
                    vacuno_id = entity.id,
                    estado_code = "MUERTO",
                    fecha_estado = DateOnly.FromDateTime(now),
                    created_at = now
                };
                _ganaderiaDbContext.vacuno_estado_historials.Add(est);
            }
        }

        await _ganaderiaDbContext.SaveChangesAsync(cancellationToken);
        return ToDomain(entity);
    }

    public async Task<(List<VacunoListItem> Items, int TotalCount)> GetPagedAsync(
    string? query, DateTime? fechaDesde, DateTime? fechaHasta, string? estado,
    int page, int limit, CancellationToken cancellationToken = default)
    {
        var q = _ganaderiaDbContext.vacunos
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(estado))
        {
            var estadoNormalizado = estado.Trim().ToLower();
            if (estadoNormalizado == "vivo")
                q = q.Where(v => v.deleted_at == null);
            else if (estadoNormalizado == "muerto")
                q = q.Where(v => v.deleted_at != null);
            // cualquier otro valor no reconocido: no se aplica filtro, se comporta como "todos"
        }
        // si estado es null o "" -> no se filtra, trae vivos y muertos

        if (!string.IsNullOrWhiteSpace(query))
        {
            var pattern = $"%{query}%";
            q = q.Where(v => EF.Functions.Like(v.codigo, pattern)
                          || EF.Functions.Like(v.nombre, pattern));
        }

        if (fechaDesde.HasValue)
            q = q.Where(v => v.fecha_registro >= DateOnly.FromDateTime(fechaDesde.Value));

        if (fechaHasta.HasValue)
            q = q.Where(v => v.fecha_registro <= DateOnly.FromDateTime(fechaHasta.Value));

        var totalCount = await q.CountAsync(cancellationToken);

        var rows = await q
            .OrderByDescending(v => v.fecha_registro)
            .ThenByDescending(v => v.id)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(v => new
            {
                v.id,
                v.codigo,
                v.nombre,
                v.fecha_nacimiento,
                v.raza_code,
                v.deleted_at,
                v.fecha_registro,
                GranjaNombre = v.granja!.nombre,
                DistritoNombre = v.granja!.distrito_codigoNavigation!.nombre,
                ProvinciaNombre = v.granja!.distrito_codigoNavigation!.provincia_codigoNavigation!.nombre,
                DepartamentoNombre = v.granja!.distrito_codigoNavigation!
                                        .provincia_codigoNavigation!
                                        .departamento_codigoNavigation!.nombre
            })
            .ToListAsync(cancellationToken);

        var items = rows.Select(r =>
        {
            var procedencia = string.Join(", ",
                new[] { r.GranjaNombre, r.DistritoNombre, r.ProvinciaNombre, r.DepartamentoNombre }
                .Where(s => !string.IsNullOrWhiteSpace(s)));

            return new VacunoListItem(
                Id: r.id,
                Codigo: r.codigo,
                Nombre: r.nombre,
                FechaNacimiento: r.fecha_nacimiento,
                RazaCode: r.raza_code ?? string.Empty,
                Procedencia: string.IsNullOrWhiteSpace(procedencia) ? null : procedencia,
                IsDeleted: r.deleted_at != null,
                FechaRegistro: r.fecha_registro
            );
        }).ToList();

        return (items, totalCount);
    }

    public async Task<List<VacunoGenealogiaNode>> GetArbolGenealogicoAsync(
    long id, int maxNiveles, CancellationToken cancellationToken = default)
    {
        var nivelPorId = new Dictionary<long, int>();
        var resultado = new List<VacunoGenealogiaNode>();
        var idsNivelActual = new List<long> { id };
        var nivel = 1;

        while (idsNivelActual.Count > 0 && nivel <= maxNiveles)
        {
            var entidadesNivel = await _ganaderiaDbContext.vacunos
                .AsNoTracking()
                .Include(v => v.granja)
                    .ThenInclude(g => g.distrito_codigoNavigation)
                        .ThenInclude(d => d.provincia_codigoNavigation)
                            .ThenInclude(p => p.departamento_codigoNavigation)
                .Where(v => idsNivelActual.Contains(v.id) && v.deleted_at == null)
                .ToListAsync(cancellationToken);

            var siguienteNivel = new List<long>();

            foreach (var v in entidadesNivel)
            {
                if (nivelPorId.ContainsKey(v.id)) continue;

                nivelPorId[v.id] = nivel;

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
                    if (string.IsNullOrWhiteSpace(procedencia)) procedencia = null;
                }

                resultado.Add(new VacunoGenealogiaNode(ToDomain(v), nivel, procedencia));

                if (v.padre_id.HasValue) siguienteNivel.Add(v.padre_id.Value);
                if (v.madre_id.HasValue) siguienteNivel.Add(v.madre_id.Value);
            }

            idsNivelActual = siguienteNivel.Distinct().Where(i => !nivelPorId.ContainsKey(i)).ToList();
            nivel++;
        }

        return resultado.OrderBy(n => n.Nivel).ThenBy(n => n.Vacuno.Id).ToList();
    }

    public async Task<List<VacunoReferenceItem>> ListReferencesAsync(CancellationToken cancellationToken = default)
    {
        return await _ganaderiaDbContext.vacunos
            .AsNoTracking()
            .Where(v => v.deleted_at == null
                && !_ganaderiaDbContext.v_vacuno_estado_vigentes
                    .Any(e => e.vacuno_id == v.id && e.estado_code == "MUERTO"))
            .OrderBy(v => v.codigo)
            .Select(v => new VacunoReferenceItem(
                v.id,
                v.codigo,
                v.nombre,
                v.sexo_code,
                _ganaderiaDbContext.v_vacuno_estado_vigentes
                    .Where(e => e.vacuno_id == v.id)
                    .Select(e => e.estado_code)
                    .FirstOrDefault()))
            .ToListAsync(cancellationToken);
    }

    public async Task<VacunoCatalogs> GetCatalogsAsync(CancellationToken cancellationToken = default)
    {
        var tiposAdquisicion = await _ganaderiaDbContext.cat_tipo_adquisicions
            .AsNoTracking().Where(item => item.activo).OrderBy(item => item.nombre)
            .Select(item => new VacunoCatalogOption(item.code, item.nombre)).ToListAsync(cancellationToken);

        var razas = await _ganaderiaDbContext.cat_razas
            .AsNoTracking().Where(item => item.activo).OrderBy(item => item.nombre)
            .Select(item => new VacunoCatalogOption(item.code, item.nombre)).ToListAsync(cancellationToken);

        var colores = await _ganaderiaDbContext.cat_colors
            .AsNoTracking().Where(item => item.activo).OrderBy(item => item.nombre)
            .Select(item => new VacunoCatalogOption(item.code, item.nombre)).ToListAsync(cancellationToken);

        var sexos = await _ganaderiaDbContext.cat_sexos
            .AsNoTracking().OrderBy(item => item.nombre)
            .Select(item => new VacunoCatalogOption(item.code, item.nombre)).ToListAsync(cancellationToken);

        var estados = await _ganaderiaDbContext.cat_estado_vacunos
            .AsNoTracking().OrderBy(item => item.nombre)
            .Select(item => new VacunoCatalogOption(item.code, item.nombre)).ToListAsync(cancellationToken);

        var utilizaciones = await _ganaderiaDbContext.cat_tipo_utilizacions
            .AsNoTracking().Where(item => item.activo).OrderBy(item => item.nombre)
            .Select(item => new VacunoCatalogOption(item.code, item.nombre)).ToListAsync(cancellationToken);

        var granjas = await _ganaderiaDbContext.granjas
            .AsNoTracking().Where(item => item.activo).OrderBy(item => item.nombre)
            .Select(item => new GranjaCatalogOption(item.id, item.nombre)).ToListAsync(cancellationToken);

        return new VacunoCatalogs(tiposAdquisicion, razas, colores, sexos, estados, utilizaciones, granjas);
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
