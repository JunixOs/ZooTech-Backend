using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Modules.Module_Vacuno.Common;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Services;

public sealed class VacunoResponseEnricher : IVacunoResponseEnricher
{
    private readonly GanaderiaDbContext _db;

    public VacunoResponseEnricher(GanaderiaDbContext db)
    {
        _db = db;
    }

    public async Task<VacunoResponse> EnrichAsync(
        VacunoOutput dto,
        CancellationToken cancellationToken = default)
    {
        var parentIds = new[] { dto.PadreId, dto.MadreId }
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();

        var parentCodes = parentIds.Count == 0
            ? new Dictionary<long, string>()
            : await _db.vacunos
                .AsNoTracking()
                .Where(v => parentIds.Contains(v.id) && v.deleted_at == null)
                .ToDictionaryAsync(v => v.id, v => v.codigo, cancellationToken);

        var granja = await _db.granjas
            .AsNoTracking()
            .Where(g => g.id == dto.GranjaId)
            .Select(g => new
            {
                g.nombre,
                CodigoDistrito = g.distrito_codigo,
                Distrito = g.distrito_codigoNavigation != null ? g.distrito_codigoNavigation.nombre : null,
                Provincia = g.distrito_codigoNavigation != null && g.distrito_codigoNavigation.provincia_codigoNavigation != null
                    ? g.distrito_codigoNavigation.provincia_codigoNavigation.nombre
                    : null,
                Departamento = g.distrito_codigoNavigation != null
                    && g.distrito_codigoNavigation.provincia_codigoNavigation != null
                    && g.distrito_codigoNavigation.provincia_codigoNavigation.departamento_codigoNavigation != null
                        ? g.distrito_codigoNavigation.provincia_codigoNavigation.departamento_codigoNavigation.nombre
                        : null
            })
            .FirstOrDefaultAsync(cancellationToken);

        var adquisicion = await _db.vacuno_adquisicions
            .AsNoTracking()
            .Where(a => a.vacuno_id == dto.Id)
            .Select(a => new
            {
                a.precio_compra
            })
            .FirstOrDefaultAsync(cancellationToken);

        var utilizacion = await _db.vacuno_utilizacion_historials
            .AsNoTracking()
            .Where(u => u.vacuno_id == dto.Id)
            .OrderByDescending(u => u.created_at)
            .Select(u => new
            {
                u.tipo_utilizacion_code,
                u.created_at
            })
            .FirstOrDefaultAsync(cancellationToken);

        return new VacunoResponse(
            dto.Id,
            dto.Codigo,
            dto.Nombre,
            dto.FechaNacimiento,
            dto.TipoAdquisicionCode,
            dto.RazaCode,
            dto.ColorCode,
            dto.SexoCode,
            dto.PadreId,
            dto.MadreId,
            dto.GranjaId,
            dto.Observaciones,
            dto.FechaRegistro,
            dto.CreatedAt,
            dto.UpdatedAt,
            dto.PadreId.HasValue && parentCodes.TryGetValue(dto.PadreId.Value, out var codigoPadre) ? codigoPadre : null,
            dto.MadreId.HasValue && parentCodes.TryGetValue(dto.MadreId.Value, out var codigoMadre) ? codigoMadre : null,
            granja?.nombre,
            granja?.Distrito,
            granja?.Provincia,
            granja?.Departamento,
            granja?.CodigoDistrito,
            adquisicion?.precio_compra,
            utilizacion?.tipo_utilizacion_code,
            utilizacion is null ? null : DateOnly.FromDateTime(utilizacion.created_at));
    }
}
