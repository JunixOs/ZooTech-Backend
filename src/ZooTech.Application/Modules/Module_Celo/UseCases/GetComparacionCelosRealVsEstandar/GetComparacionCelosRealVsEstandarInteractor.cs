using ZooTech.Domain.Module_Celo.Interfaces;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetComparacionCelosRealVsEstandar;

public sealed class GetComparacionCelosRealVsEstandarInteractor
    : IGetComparacionCelosRealVsEstandarInputPort
{
    private readonly ICeloRepository _repository;

    public GetComparacionCelosRealVsEstandarInteractor(
        ICeloRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetComparacionCelosRealVsEstandarOutput> HandleAsync(
        DateTime? fechaInicio,
        DateTime? fechaFin,
        CancellationToken cancellationToken = default)
    {
        var registros = await _repository.GetByDateRangeAsync(
            fechaInicio,
            fechaFin,
            cancellationToken);

        var promedio = registros.Any()
            ? (int)Math.Round(registros.Count / (double)registros
                .GroupBy(x => DateOnly.FromDateTime(x.FechaHora))
                .Count())
            : 0;

        var items = registros
            .GroupBy(x => DateOnly.FromDateTime(x.FechaHora))
            .OrderBy(x => x.Key)
            .Select(x => new ComparacionCelosItemDto
            {
                Fecha = x.Key,
                RegistrosReales = x.Count(),
                RegistrosEstandar = promedio
            })
            .ToList();

        return new GetComparacionCelosRealVsEstandarOutput(items);
    }
}