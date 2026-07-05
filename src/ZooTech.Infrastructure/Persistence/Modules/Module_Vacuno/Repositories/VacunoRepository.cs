using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Modules.Module_Vacuno.Common;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GenerarArbolGenealogico;
using ZooTech.Domain.Module_Vacuno.Entities;
using ZooTech.Domain.Module_Vacuno.Interfaces;
using ZooTech.Domain.Module_Vacuno.Models;
using ZooTech.Domain.Module_Vacuno.ReadModels;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Modules.Module_Vacuno.Mappers;

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

        var estados = await _context.cat_estado_vacunos
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

        return new VacunoCatalogs(tiposAdquisicion, razas, colores, sexos, estados, utilizaciones, granjas);
    }

    public async Task<Vacuno> AddAsync(Vacuno vacuno, decimal? precioCompra, string? aptoPara, CancellationToken cancellationToken = default)
    {
        var entity = VacunoPersistenceMapper.ToEntity(vacuno);
        _context.vacunos.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        var now = DateTime.UtcNow;

        // Persistir adquisición si aplica
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
            _context.vacuno_adquisicions.Add(adq);
        }

        // Persistir utilización (aptoPara)
        if (!string.IsNullOrEmpty(aptoPara))
        {
            var util = new ZooTech.Infrastructure.Persistence.Entities.vacuno_utilizacion_historial
            {
                vacuno_id = entity.id,
                tipo_utilizacion_code = aptoPara,
                created_at = now
            };
            _context.vacuno_utilizacion_historials.Add(util);
        }

        // Persistir estado inicial del catalogo oficial.
        var est = new ZooTech.Infrastructure.Persistence.Entities.vacuno_estado_historial
        {
            vacuno_id = entity.id,
            estado_code = "SANO",
            fecha_estado = DateOnly.FromDateTime(now),
            created_at = now
        };
        _context.vacuno_estado_historials.Add(est);

        await _context.SaveChangesAsync(cancellationToken);
        return VacunoPersistenceMapper.ToDomain(entity);
    }

    public async Task<Vacuno> UpdateAsync(Vacuno vacuno, decimal? precioCompra, string? aptoPara, CancellationToken cancellationToken = default)
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

        var now = DateTime.UtcNow;

        // Actualizar precio de compra
        var existingAdq = await _context.vacuno_adquisicions.FirstOrDefaultAsync(a => a.vacuno_id == entity.id, cancellationToken);
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
            _context.vacuno_adquisicions.Add(adq);
        }

        // Actualizar utilización (aptoPara)
        var currentUtil = await _context.vacuno_utilizacion_historials
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
                _context.vacuno_utilizacion_historials.Add(util);
            }
        }

        // Si se realiza borrado lógico, registrar en el historial de estados
        if (vacuno.IsDeleted)
        {
            var existingDeletedState = await _context.vacuno_estado_historials
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
                _context.vacuno_estado_historials.Add(est);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return VacunoPersistenceMapper.ToDomain(entity);
    }

    public Task<VacunoNodoDto?> GetArbolGenealogicoAsync(long vacunoId, int niveles)
    {
        throw new NotImplementedException("El árbol genealógico utiliza el repositorio principal de lectura.");
    }

    public async Task<VacunoDetalleDto?> GetDetalleByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.vacunos
            .AsNoTracking()
            .Where(v => v.id == id && v.deleted_at == null)
            .Select(v => new VacunoDetalleDto
            {
                Id = v.id,
                Codigo = v.codigo,
                Nombre = v.nombre,
                FechaNacimiento = v.fecha_nacimiento,
                AdquisicionPor = v.tipo_adquisicion_codeNavigation.nombre,
                PrecioCompra = v.vacuno_adquisicion != null ? v.vacuno_adquisicion.precio_compra : null,
                Raza = v.raza_codeNavigation.nombre,
                Color = v.color_codeNavigation.nombre,
                Sexo = v.sexo_codeNavigation.nombre,
                CodigoPadre = v.padre != null ? v.padre.codigo : null,
                CodigoMadre = v.madre != null ? v.madre.codigo : null,
                Granja = v.granja.nombre,
                Distrito = v.granja.distrito_codigoNavigation != null ? v.granja.distrito_codigoNavigation.nombre : null,
                Provincia = v.granja.distrito_codigoNavigation != null && v.granja.distrito_codigoNavigation.provincia_codigoNavigation != null
                    ? v.granja.distrito_codigoNavigation.provincia_codigoNavigation.nombre : null,
                Departamento = v.granja.distrito_codigoNavigation != null && v.granja.distrito_codigoNavigation.provincia_codigoNavigation != null
                    && v.granja.distrito_codigoNavigation.provincia_codigoNavigation.departamento_codigoNavigation != null
                        ? v.granja.distrito_codigoNavigation.provincia_codigoNavigation.departamento_codigoNavigation.nombre : null,
                CodigoDistrito = v.granja.distrito_codigo,
                AptoPara = v.vacuno_utilizacion_historials.OrderByDescending(u => u.created_at).Select(u => u.tipo_utilizacion_code).FirstOrDefault(),
                FechaEspecificacion = v.vacuno_utilizacion_historials.OrderByDescending(u => u.created_at).Select(u => (DateOnly?)DateOnly.FromDateTime(u.created_at)).FirstOrDefault(),
                Observaciones = v.observaciones,
                FotoUrl = v.vacuno_foto != null && v.vacuno_foto.archivo != null ? v.vacuno_foto.archivo.ruta_archivo : null,
                Estado = v.vacuno_estado_historials.OrderByDescending(eh => eh.fecha_estado).ThenByDescending(eh => eh.id).Select(eh => eh.estado_code).FirstOrDefault() ?? "SANO",
                CreadoEn = v.created_at,
                ActualizadoEn = v.updated_at,
                PadreId = v.padre_id,
                MadreId = v.madre_id,
                GranjaId = v.granja_id
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
