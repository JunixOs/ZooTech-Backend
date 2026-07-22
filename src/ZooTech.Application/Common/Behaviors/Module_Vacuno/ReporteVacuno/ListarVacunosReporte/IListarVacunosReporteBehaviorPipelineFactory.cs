using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ListarVacunosReporte;

namespace ZooTech.Application.Common.Behaviors.Module_Vacuno.ReporteVacuno.ListarVacunosReporte
{
    public interface IListarVacunosReporteBehaviorPipelineFactory
    {
        BehaviorPipeline<ListarVacunosReporteQuery , ListarVacunosReporteResponse> Create();
    }
}