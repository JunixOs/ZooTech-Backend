using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetPesoPromedioReport;

public sealed class GetPesoPromedioReportInteractor : IGetPesoPromedioReportInputPort
{
    private readonly ITriajeRepository _repository;

    public GetPesoPromedioReportInteractor(ITriajeRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetPesoPromedioReportOutput> HandleAsync(int meses, CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetPesoPromedioPorPeriodoAsync(meses, cancellationToken);
        return new GetPesoPromedioReportOutput(
            items.Select(i => new PesoPromedioItemOutput(i.Anio, i.Mes, i.PesoPromedio))
                .ToList().AsReadOnly());
    }
}
