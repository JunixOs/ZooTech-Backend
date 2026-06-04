using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Ports;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Reportes.GetReporteDiario;

public class GetReporteDiarioInteractor : IGetReporteDiarioInputPort
{
    private readonly IOrdenioRepository _repository;

    public GetReporteDiarioInteractor(IOrdenioRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetReporteDiarioOutput> HandleAsync(GetReporteDiarioQuery query, CancellationToken cancellationToken)
    {
        // Llamamos al repositorio para obtener la agregación por día
        var items = await _repository.GetProduccionDiariaAsync(query.FechaDesde, query.FechaHasta, query.VacunoId, cancellationToken);

        // Mapear a output
        var dtoItems = items.Select(x => new ReporteDiarioItem(x.Fecha.Date, x.TotalLitros, x.CantidadOrdenios)).ToList();
        return new GetReporteDiarioOutput(dtoItems);
    }
}
