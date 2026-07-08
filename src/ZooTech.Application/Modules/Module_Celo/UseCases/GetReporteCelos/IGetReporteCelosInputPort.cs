using ZooTech.Application.Common.Models;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetReporteCelos;

public interface IGetReporteCelosInputPort
{
    Task<GetReporteCelosOutput> HandleAsync(EmptyCommand emptyCommand, CancellationToken cancellationToken = default);
}
