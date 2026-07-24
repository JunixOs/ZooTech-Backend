using ZooTech.Application.Common.Gateway.Reports;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarActividadVacunos;

namespace ZooTech.Application.Common.Behaviors.Module_Vacuno.ExportarActividadVacunos;

public sealed class ExportarActividadVacunosBehaviorPipelineFactory
    : IExportarActividadVacunosBehaviorPipelineFactory
{
    private readonly ValidationBehavior<ExportarActividadVacunosQuery, GeneratedReportDocument> _validation;
    private readonly LoggingBehavior<ExportarActividadVacunosQuery, GeneratedReportDocument> _logging;
    private readonly AuditBehavior<ExportarActividadVacunosQuery, GeneratedReportDocument> _audit;
    private readonly IExportarActividadVacunosInputPort _handler;

    public ExportarActividadVacunosBehaviorPipelineFactory(
        ValidationBehavior<ExportarActividadVacunosQuery, GeneratedReportDocument> validation,
        LoggingBehavior<ExportarActividadVacunosQuery, GeneratedReportDocument> logging,
        AuditBehavior<ExportarActividadVacunosQuery, GeneratedReportDocument> audit,
        IExportarActividadVacunosInputPort handler)
    {
        _validation = validation;
        _logging = logging;
        _audit = audit;
        _handler = handler;
    }

    public BehaviorPipeline<ExportarActividadVacunosQuery, GeneratedReportDocument> Create()
        => new(
            [_validation, _logging, _audit],
            _handler.HandleAsync);
}
