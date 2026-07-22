using ZooTech.Application.Modules.Module_Celo.UseCases.ListReporteCeloGeneral;

namespace ZooTech.Application.Common.Behaviors.Module_Celo.ListReporteCeloGeneral
{
    public interface IListReporteCeloGeneralBehaviorPipelineFactory
    {
        BehaviorPipeline<ListReporteCeloGeneralCommand , ListReporteCeloGeneralOutput> Create();
    }
}