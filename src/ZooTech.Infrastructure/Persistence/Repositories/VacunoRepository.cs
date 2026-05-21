using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Common.Gateway.Repositories;
using ZooTech.Application.Modules.Module_Vacunos.UseCases.ListarVacunos;
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
}
