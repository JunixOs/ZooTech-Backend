using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.ListReporteCeloGeneral;

public interface IListReporteCeloGeneralInputPort
    : IRequestHandler<ListReporteCeloGeneralQuery , ListReporteCeloGeneralOutput>
{
}
