using ZooTech.Domain.Module_Celo.Interfaces;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetComparacionCelosRealVsEstandarPorVacuno;

public sealed class GetComparacionCelosRealVsEstandarPorVacunoInteractor
    : IGetComparacionCelosRealVsEstandarPorVacunoInputPort
{
    private const int DuracionCicloDias = 21;

    private readonly ICeloRepository _repository;

    public GetComparacionCelosRealVsEstandarPorVacunoInteractor(
        ICeloRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetComparacionCelosRealVsEstandarPorVacunoOutput> HandleAsync(
        GetComparacionCelosRealVsEstandarPorVacunoQuery query,
        CancellationToken cancellationToken = default)
    {
        var celos = await _repository.GetAllAsync(cancellationToken);

        var fechaFinInclusive = query.FechaFin?.Date.AddDays(1).AddTicks(-1);

        var registrosVacuno = celos
            .Where(c => c.VacunoCodigo == query.CodigoVacuno)
            .Where(c =>
                (!query.FechaInicio.HasValue || c.FechaHora >= query.FechaInicio.Value) &&
                (!fechaFinInclusive.HasValue || c.FechaHora <= fechaFinInclusive.Value))
            .Select(c => c.FechaHora)
            .ToList();

        if (registrosVacuno.Count == 0)
        {
            return new GetComparacionCelosRealVsEstandarPorVacunoOutput(
                new List<ComparacionCelosPorVacunoItemDto>());
        }

        var inicioRango = DateOnly.FromDateTime(
            query.FechaInicio ?? registrosVacuno.Min());
        var finRango = DateOnly.FromDateTime(
            query.FechaFin ?? registrosVacuno.Max());

        var registrosPorMes = registrosVacuno
            .GroupBy(f => new DateOnly(f.Year, f.Month, 1))
            .ToDictionary(g => g.Key, g => g.Count());

        var items = new List<ComparacionCelosPorVacunoItemDto>();
        var mesActual = new DateOnly(inicioRango.Year, inicioRango.Month, 1);
        var mesFinal = new DateOnly(finRango.Year, finRango.Month, 1);

        while (mesActual <= mesFinal)
        {
            var diasEnMes = DateTime.DaysInMonth(mesActual.Year, mesActual.Month);
            var registrosEstandar = (int)Math.Round(
                diasEnMes / (double)DuracionCicloDias,
                MidpointRounding.AwayFromZero);

            items.Add(new ComparacionCelosPorVacunoItemDto
            {
                Periodo = mesActual,
                RegistrosReales = registrosPorMes.GetValueOrDefault(mesActual, 0),
                RegistrosEstandar = registrosEstandar,
            });

            mesActual = mesActual.AddMonths(1);
        }

        return new GetComparacionCelosRealVsEstandarPorVacunoOutput(items);
    }
}
