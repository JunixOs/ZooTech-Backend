namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetPesoPromedioReport;

public interface IGetPesoPromedioReportInputPort
{
    Task<GetPesoPromedioReportOutput> HandleAsync(int meses, CancellationToken cancellationToken = default);
}
