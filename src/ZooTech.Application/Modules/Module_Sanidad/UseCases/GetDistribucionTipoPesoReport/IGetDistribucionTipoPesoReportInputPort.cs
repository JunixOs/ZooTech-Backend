namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetDistribucionTipoPesoReport;

public interface IGetDistribucionTipoPesoReportInputPort
{
    Task<GetDistribucionTipoPesoReportOutput> HandleAsync(CancellationToken cancellationToken = default);
}
