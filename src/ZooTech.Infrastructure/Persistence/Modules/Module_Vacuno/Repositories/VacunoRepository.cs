using Microsoft.EntityFrameworkCore;
using ZooTech.Domain.Module_Vacuno.Entities;
using ZooTech.Domain.Module_Vacuno.Interfaces;
using ZooTech.Infrastructure.Persistence.Context;

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

    public async Task<List<(Vacuno Vacuno, string? Procedencia)>> ListAllForDisplayAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _ganaderiaDbContext.vacunos
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
        var entity = await _ganaderiaDbContext.vacunos
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.id == id && v.deleted_at == null, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default)
        => _ganaderiaDbContext.vacunos
            .AnyAsync(v => v.id == id && v.deleted_at == null, cancellationToken);

    public Task<bool> ExistsCodigoAsync(string codigo, CancellationToken cancellationToken = default)
        => _ganaderiaDbContext.vacunos.AnyAsync(v => v.deleted_at == null && v.codigo == codigo.Trim(), cancellationToken);

    public async Task<Vacuno> AddAsync(Vacuno vacuno, CancellationToken cancellationToken = default)
    {
        var entity = ToEntity(vacuno);
        _ganaderiaDbContext.vacunos.Add(entity);
        await _ganaderiaDbContext.SaveChangesAsync(cancellationToken);
        return ToDomain(entity);
    }

    public async Task<Vacuno> UpdateAsync(Vacuno vacuno, CancellationToken cancellationToken = default)
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

        await _ganaderiaDbContext.SaveChangesAsync(cancellationToken);
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
