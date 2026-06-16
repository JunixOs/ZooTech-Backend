using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetDistribucionTipoPesoReport;

public sealed class GetDistribucionTipoPesoReportInteractor : IGetDistribucionTipoPesoReportInputPort
{
    private readonly ITriajeRepository _repository;

    public GetDistribucionTipoPesoReportInteractor(ITriajeRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetDistribucionTipoPesoReportOutput> HandleAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetDistribucionTipoPesoAsync(cancellationToken);
        return new GetDistribucionTipoPesoReportOutput(
            items.Select(i => new DistribucionItemOutput(i.TipoPesoCode, i.Cantidad, i.PesoPromedio))
                .ToList().AsReadOnly());
    }
}
