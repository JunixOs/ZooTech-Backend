using Microsoft.EntityFrameworkCore;
using System.Linq;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetArbolGenealogico;
using ZooTech.Domain.Module_Vacuno.Entities;
using ZooTech.Domain.Module_Vacuno.Interfaces;
using ZooTech.Domain.Module_Vacuno.ReadModels.GetArbolGenealogico;
using ZooTech.Domain.Module_Vacuno.ReadModels.ListarVacuno;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Mappers;
using ZooTech.Infrastructure.Persistence.Models;

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

    public async Task<(List<VacunoListItem> Items, int TotalCount)> GetPagedAsync(
    string? query, DateTime? fechaDesde, DateTime? fechaHasta, string? estado,
    int page, int limit, CancellationToken cancellationToken = default)
    {
        var q = _context.vacunos
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
            var entidadesNivel = await _context.vacunos
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
}
