using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Common.Models;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetReporteCelos;

public interface IGetReporteCelosInputPort : IRequestHandler<EmptyCommandQuery , GetReporteCelosOutput>
{
}
