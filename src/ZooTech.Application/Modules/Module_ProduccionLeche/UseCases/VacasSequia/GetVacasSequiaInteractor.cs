using ZooTech.Domain.Module_ProduccionLeche.Interfaces;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.VacasSequia;

public sealed class GetVacasSequiaInteractor : IGetVacasSequiaInputPort
{
    private readonly IPeriodoSequiaRepository _repository;

    public GetVacasSequiaInteractor(IPeriodoSequiaRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetVacasSequiaOutput> HandleAsync(CancellationToken cancellationToken)
    {
        var result = await _repository.GetVacasSequiaAsync(cancellationToken);

        return new GetVacasSequiaOutput(
            result.Actuales.Select(a => new VacaSequiaActualDto(a.CodigoVacuno, a.NombreVacuno, a.Desde, a.ProbablementeHasta)).ToList(),
            result.Futuros.Select(f => new VacaSequiaFuturoDto(f.CodigoVacuno, f.NombreVacuno, f.ProbablementeDesde, f.ProbablementeHasta)).ToList(),
            result.Historicos.Select(h => new VacaSequiaHistoricoDto(h.CodigoVacuno, h.NombreVacuno, h.Desde, h.Hasta)).ToList());
    }
}
