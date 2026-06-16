using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetResumenReport;

public sealed class GetResumenReportInteractor : IGetResumenReportInputPort
{
    private readonly ITriajeRepository _repository;

    public GetResumenReportInteractor(ITriajeRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetResumenReportOutput> HandleAsync(CancellationToken cancellationToken = default)
    {
        var resumen = await _repository.GetResumenAsync(cancellationToken);
        return new GetResumenReportOutput(
            resumen.TotalTriajes,
            resumen.TotalVacunosConTriajes,
            resumen.PesoPromedioGeneral,
            resumen.DistribucionTipoPeso
                .Select(d => new TipoPesoReportItem(d.TipoPesoCode, d.Cantidad, d.PesoPromedio))
                .ToList().AsReadOnly());
    }
}
