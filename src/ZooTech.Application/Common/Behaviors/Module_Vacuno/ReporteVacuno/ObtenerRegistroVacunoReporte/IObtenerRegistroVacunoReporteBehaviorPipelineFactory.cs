using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte;

namespace ZooTech.Application.Common.Behaviors.Module_Vacuno.ReporteVacuno.ObtenerRegistroVacunoReporte
{
    public interface IObtenerRegistroVacunoReporteBehaviorPipelineFactory
    {
        BehaviorPipeline<ObtenerRegistroVacunoReporteQuery , RegistroVacunoReporteResponse> Create();
    }
}