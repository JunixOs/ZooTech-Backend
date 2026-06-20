using Microsoft.Data.SqlClient;
using ZooTech.Domain.Module_Vacuno.Criteria;
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

    public async Task<(IReadOnlyCollection<Vacuno> Items, int TotalRegistros)> ListarAvanzadoAsync(
        ListarVacunosCriteriaDomain criteria,
        CancellationToken cancellationToken = default)
    {
        var offset = (criteria.Page - 1) * criteria.Limit;

        var query = _context.vacunos
            .AsNoTracking()
            .Where(v => v.deleted_at == null);

        if (criteria.FechaDesde.HasValue && criteria.FechaHasta.HasValue)
        {
            var fechaDesde = DateOnly.FromDateTime(criteria.FechaDesde.Value.ToDateTime(TimeOnly.MinValue));
            var fechaHasta = DateOnly.FromDateTime(criteria.FechaHasta.Value.ToDateTime(TimeOnly.MinValue));
            query = query.Where(v => v.fecha_registro >= fechaDesde && v.fecha_registro <= fechaHasta);
        }

        if (!string.IsNullOrWhiteSpace(criteria.Q))
        {
            var q = criteria.Q.Trim();
            query = query.Where(v => v.codigo.Contains(q) || v.nombre.Contains(q));
        }

        if (!string.IsNullOrWhiteSpace(criteria.Raza))
        {
            var raza = criteria.Raza.Trim();
            query = query.Where(v => v.raza_codeNavigation != null && v.raza_codeNavigation.nombre.Contains(raza));
        }

        if (!string.IsNullOrWhiteSpace(criteria.Procedencia))
        {
            var procedencia = criteria.Procedencia.Trim();
            query = query.Where(v => v.vacuno_adquisicion != null && v.vacuno_adquisicion.proveedor != null && v.vacuno_adquisicion.proveedor.Contains(procedencia));
        }

        if (!string.IsNullOrWhiteSpace(criteria.Estado))
        {
            var estado = criteria.Estado.Trim();
            string[] estadoCodes = Array.Empty<string>();
            var lowerEstado = estado.ToLowerInvariant();
            if (lowerEstado == "vivo") estadoCodes = ["ACTIVO", "VIVO"];
            else if (lowerEstado == "muerto") estadoCodes = ["MUERTO", "FALLECIDO", "BAJA"];

            query = query.Where(v => 
                v.vacuno_estado_historials.OrderByDescending(h => h.id).FirstOrDefault() != null && (
                v.vacuno_estado_historials.OrderByDescending(h => h.id).FirstOrDefault()!.estado_code == estado ||
                v.vacuno_estado_historials.OrderByDescending(h => h.id).FirstOrDefault()!.estado_codeNavigation.nombre == estado ||
                (estadoCodes.Length > 0 && estadoCodes.Contains(v.vacuno_estado_historials.OrderByDescending(h => h.id).FirstOrDefault()!.estado_code))
                ));
        }

        if (!string.IsNullOrWhiteSpace(criteria.AptoPara))
        {
            var aptoPara = criteria.AptoPara.Trim();
            query = query.Where(v => 
                v.vacuno_utilizacion_historials.OrderByDescending(h => h.id).FirstOrDefault() != null && (
                v.vacuno_utilizacion_historials.OrderByDescending(h => h.id).FirstOrDefault()!.tipo_utilizacion_code == aptoPara ||
                v.vacuno_utilizacion_historials.OrderByDescending(h => h.id).FirstOrDefault()!.tipo_utilizacion_codeNavigation.nombre.Contains(aptoPara)
                ));
        }

        var total = await query.CountAsync(cancellationToken);

        var pagedEntities = await query
            .OrderByDescending(v => v.fecha_registro)
            .ThenBy(v => v.codigo)
            .Skip(offset)
            .Take(criteria.Limit)
            .ToListAsync(cancellationToken);

        var items = pagedEntities.Select(ToDomain).ToList();

        return (items, total);
    }

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
}
