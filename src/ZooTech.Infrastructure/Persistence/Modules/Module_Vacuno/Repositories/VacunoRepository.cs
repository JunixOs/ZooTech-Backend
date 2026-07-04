using Microsoft.EntityFrameworkCore;
using ZooTech.Domain.Module_Vacuno.Entities;
using ZooTech.Domain.Module_Vacuno.Interfaces;
using ZooTech.Domain.Module_Vacuno.Models;
using ZooTech.Domain.Module_Vacuno.ReadModels;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Modules.Module_Vacuno.Mappers;

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

        return entities.Select(VacunoPersistenceMapper.ToDomain).ToList();
    }

    public async Task<List<Vacuno>> ListAllWithDeletedAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.vacunos
            .AsNoTracking()
            .OrderBy(v => v.codigo)
            .ToListAsync(cancellationToken);

        return entities.Select(VacunoPersistenceMapper.ToDomain).ToList();
    }

    public async Task<List<VacunoListItem>> ListAllForDisplayAsync(CancellationToken cancellationToken = default)
    {
        return await _context.vacunos
            .AsNoTracking()
            .OrderBy(v => v.codigo)
            .Select(v => new VacunoListItem(
                v.id,
                v.codigo,
                v.nombre,
                v.fecha_nacimiento,
                v.fecha_registro,
                v.raza_code,
                v.granja == null
                    ? null
                    : v.granja.nombre + ", "
                        + v.granja.distrito_codigoNavigation.nombre + ", "
                        + v.granja.distrito_codigoNavigation.provincia_codigoNavigation.nombre + ", "
                        + v.granja.distrito_codigoNavigation.provincia_codigoNavigation.departamento_codigoNavigation.nombre,
                v.deleted_at != null))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<VacunoReferenceItem>> ListReferencesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.vacunos
            .AsNoTracking()
            .Where(v => v.deleted_at == null)
            .OrderBy(v => v.codigo)
            .Select(v => new VacunoReferenceItem(
                v.id,
                v.codigo,
                v.nombre,
                v.sexo_code))
            .ToListAsync(cancellationToken);
    }

    public async Task<Vacuno?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.vacunos
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.id == id && v.deleted_at == null, cancellationToken);

        return entity is null ? null : VacunoPersistenceMapper.ToDomain(entity);
    }

    public async Task<Vacuno?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken = default)
    {
        var entity = await _context.vacunos
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.codigo == codigo.Trim() && v.deleted_at == null, cancellationToken);

        return entity is null ? null : VacunoPersistenceMapper.ToDomain(entity);
    }

    public async Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.vacunos
            .AnyAsync(v => v.id == id && v.deleted_at == null, cancellationToken);
    }

    public Task<bool> ExistsCodigoAsync(string codigo, CancellationToken cancellationToken = default)
        => _context.vacunos.AnyAsync(v => v.deleted_at == null && v.codigo == codigo.Trim(), cancellationToken);

    public async Task<VacunoCatalogs> GetCatalogsAsync(CancellationToken cancellationToken = default)
    {
        var tiposAdquisicion = await _context.cat_tipo_adquisicions
            .AsNoTracking()
            .Where(item => item.activo)
            .OrderBy(item => item.nombre)
            .Select(item => new VacunoCatalogOption(item.code, item.nombre))
            .ToListAsync(cancellationToken);

        var razas = await _context.cat_razas
            .AsNoTracking()
            .Where(item => item.activo)
            .OrderBy(item => item.nombre)
            .Select(item => new VacunoCatalogOption(item.code, item.nombre))
            .ToListAsync(cancellationToken);

        var colores = await _context.cat_colors
            .AsNoTracking()
            .Where(item => item.activo)
            .OrderBy(item => item.nombre)
            .Select(item => new VacunoCatalogOption(item.code, item.nombre))
            .ToListAsync(cancellationToken);

        var sexos = await _context.cat_sexos
            .AsNoTracking()
            .OrderBy(item => item.nombre)
            .Select(item => new VacunoCatalogOption(item.code, item.nombre))
            .ToListAsync(cancellationToken);

        var utilizaciones = await _context.cat_tipo_utilizacions
            .AsNoTracking()
            .Where(item => item.activo)
            .OrderBy(item => item.nombre)
            .Select(item => new VacunoCatalogOption(item.code, item.nombre))
            .ToListAsync(cancellationToken);

        var granjas = await _context.granjas
            .AsNoTracking()
            .Where(item => item.activo)
            .OrderBy(item => item.nombre)
            .Select(item => new GranjaCatalogOption(item.id, item.nombre))
            .ToListAsync(cancellationToken);

        return new VacunoCatalogs(tiposAdquisicion, razas, colores, sexos, utilizaciones, granjas);
    }

    public async Task<Vacuno> AddAsync(Vacuno vacuno, CancellationToken cancellationToken = default)
    {
        var entity = VacunoPersistenceMapper.ToEntity(vacuno);
        _context.vacunos.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return VacunoPersistenceMapper.ToDomain(entity);
    }

    public async Task<Vacuno> UpdateAsync(Vacuno vacuno, CancellationToken cancellationToken = default)
    {
        var entity = await _context.vacunos
            .FirstOrDefaultAsync(v => v.id == vacuno.Id, cancellationToken)
            ?? throw new InvalidOperationException("No se encontro el vacuno para actualizar.");

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
        return VacunoPersistenceMapper.ToDomain(entity);
    }
}
