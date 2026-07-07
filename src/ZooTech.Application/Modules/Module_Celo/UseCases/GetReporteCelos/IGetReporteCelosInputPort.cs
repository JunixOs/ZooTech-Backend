namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetReporteCelos;

public interface IGetReporteCelosInputPort
{
    Task<GetReporteCelosOutput> HandleAsync(CancellationToken cancellationToken = default);
}
