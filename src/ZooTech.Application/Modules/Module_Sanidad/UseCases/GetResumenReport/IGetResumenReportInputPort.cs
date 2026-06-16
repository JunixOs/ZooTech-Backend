namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetResumenReport;

public interface IGetResumenReportInputPort
{
    Task<GetResumenReportOutput> HandleAsync(CancellationToken cancellationToken = default);
}
