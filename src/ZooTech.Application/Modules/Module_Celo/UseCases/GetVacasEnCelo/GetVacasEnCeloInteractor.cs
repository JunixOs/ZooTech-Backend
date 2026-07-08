using ZooTech.Domain.Module_Celo.Interfaces;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetVacasEnCelo;

public sealed class GetVacasEnCeloInteractor : IGetVacasEnCeloInputPort
{
    private const int DuracionCicloDias = 21;
    private const int DiasEnCelo = 2;
    private const int DiasProximo = 3;

    private readonly ICeloRepository _celoRepository;

    public GetVacasEnCeloInteractor(ICeloRepository celoRepository)
    {
        _celoRepository = celoRepository;
    }

    public async Task<GetVacasEnCeloOutput> HandleAsync(
        GetVacasEnCeloCommand cmd,
        CancellationToken cancellationToken = default)
    {
        var celos = await _celoRepository.GetAllAsync(cancellationToken);
        var counts = await _celoRepository.GetVecesEnCeloCountsAsync(cancellationToken);
        var criasCounts = await _celoRepository.GetCriasCountsAsync(cancellationToken);

        var fechaFinInclusive = cmd.FechaFin?.Date.AddDays(1).AddTicks(-1);

        var filtrados = celos.Where(c =>
            (!cmd.FechaInicio.HasValue || c.FechaHora >= cmd.FechaInicio.Value) &&
            (!fechaFinInclusive.HasValue || c.FechaHora <= fechaFinInclusive.Value));

        var hoy = DateTime.UtcNow.Date;

        var items = filtrados
            .GroupBy(c => new { c.VacunoId, c.VacunoCodigo, c.NombreVacuno })
            .Select(g =>
            {
                var ultimoCelo = g.Max(c => c.FechaHora).Date;
                var diasDesdeUltimoCelo = (hoy - ultimoCelo).Days;
                var diasRestante = DuracionCicloDias - diasDesdeUltimoCelo;

                var estado = diasDesdeUltimoCelo <= DiasEnCelo
                    ? "En celo"
                    : diasRestante is >= 0 and <= DiasProximo
                        ? "Próximo"
                        : "Pasó";

                return new VacaEnCeloDto
                {
                    VacunoId = g.Key.VacunoId,
                    Codigo = g.Key.VacunoCodigo,
                    Nombre = g.Key.NombreVacuno,
                    Fecha = DateOnly.FromDateTime(ultimoCelo),
                    DiasRestante = diasRestante,
                    Estado = estado,
                    VecesEnCelo = counts.GetValueOrDefault(g.Key.VacunoId, 1),
                    Crias = criasCounts.GetValueOrDefault(g.Key.VacunoId, 0)
                };
            })
            .OrderByDescending(v => v.Fecha)
            .ToList();

        return new GetVacasEnCeloOutput(items);
    }
}
