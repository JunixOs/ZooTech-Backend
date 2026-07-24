using ZooTech.Application.Common.Gateway.Reports;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarActividadVacunos;

namespace ZooTech.Application.Common.Behaviors.Module_Vacuno.ExportarActividadVacunos;

public interface IExportarActividadVacunosBehaviorPipelineFactory
{
    BehaviorPipeline<ExportarActividadVacunosQuery, GeneratedReportDocument> Create();
}
