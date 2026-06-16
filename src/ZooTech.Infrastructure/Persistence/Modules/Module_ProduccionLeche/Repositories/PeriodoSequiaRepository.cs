using Microsoft.EntityFrameworkCore;
using ZooTech.Domain.Module_ProduccionLeche;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;
using ZooTech.Infrastructure.Persistence.Context;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_ProduccionLeche.Repositories;

public sealed class PeriodoSequiaRepository : IPeriodoSequiaRepository
{
    private readonly GanaderiaDbContext _dbContext;

    public PeriodoSequiaRepository(GanaderiaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<VacasSequiaResult> GetVacasSequiaAsync(CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var queryable = _dbContext.periodo_sequia
            .AsNoTracking()
            .Include(x => x.vacuno);

        var actuales = await queryable
            .Where(x => x.fecha_inicio <= today && x.fecha_fin_real == null)
            .Select(x => new VacaSequiaActual(
                x.vacuno.codigo,
                x.vacuno.nombre,
                x.fecha_inicio,
                x.fecha_fin_estimada))
            .ToListAsync(cancellationToken);

        var futuros = await queryable
            .Where(x => x.fecha_inicio > today)
            .Select(x => new VacaSequiaFuturo(
                x.vacuno.codigo,
                x.vacuno.nombre,
                x.fecha_inicio,
                x.fecha_fin_estimada))
            .ToListAsync(cancellationToken);

        var historicos = await queryable
            .Where(x => x.fecha_fin_real != null)
            .Select(x => new VacaSequiaHistorico(
                x.vacuno.codigo,
                x.vacuno.nombre,
                x.fecha_inicio,
                x.fecha_fin_real!.Value))
            .ToListAsync(cancellationToken);

        return new VacasSequiaResult(actuales, futuros, historicos);
    }
}
