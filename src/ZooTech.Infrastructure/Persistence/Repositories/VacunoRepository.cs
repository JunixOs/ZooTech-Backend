using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Common.Gateway.Repositories;
using ZooTech.Domain.Entities;
using ZooTech.Domain.Enums;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Mappers;

namespace ZooTech.Infrastructure.Persistence.Repositories;

public class VacunoRepository : IVacunoRepository
{
    private readonly GanaderiaDbContext _db;

    public VacunoRepository(GanaderiaDbContext db)
    {
        _db = db;
    }

    public async Task<(List<Animal> Data, int Total)> GetPagedAsync(
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        EstadoAnimal? estado,
        string? q,
        int skip,
        int take)
    {
        // Query base con navigations necesarias para el mapper
        var query = _db.vacunos
            .AsNoTracking()
            .Include(v => v.raza_codeNavigation)
            .Include(v => v.granja)
                .ThenInclude(g => g.distrito_codigoNavigation)
                    .ThenInclude(d => d.provincia_codigoNavigation)
                        .ThenInclude(p => p.departamento_codigoNavigation)
            .Where(v => v.deleted_at == null) // Solo registros activos (soft delete)
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

        // Filtro por estado usando la vista v_vacuno_estado_vigente
        if (estado.HasValue)
        {
            var estadoStr = estado.Value.ToString();
            var vacunoIdsConEstado = _db.v_vacuno_estado_vigentes
                .Where(e => e.estado_code == estadoStr)
                .Select(e => e.vacuno_id);

            query = query.Where(v => vacunoIdsConEstado.Contains(v.id));
        }

        // Filtro por búsqueda de texto (código o nombre)
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(v => v.codigo.Contains(q) || v.nombre.Contains(q));
        }

        // Total para paginación
        var total = await query.CountAsync();

        // Obtener página de datos
        var entities = await query
            .OrderByDescending(v => v.fecha_registro)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        // Obtener estados vigentes para los vacunos de esta página
        var vacunoIds = entities.Select(v => v.id).ToList();
        var estadosVigentes = await _db.v_vacuno_estado_vigentes
            .Where(e => vacunoIds.Contains(e.vacuno_id))
            .ToDictionaryAsync(e => e.vacuno_id, e => e.estado_code);

        // Mapear a dominio
        var domainEntities = entities.Select(v =>
        {
            estadosVigentes.TryGetValue(v.id, out var estadoCode);
            return VacunoMapper.ToDomain(v, estadoCode);
        }).ToList();

        return (domainEntities, total);
    }
}
