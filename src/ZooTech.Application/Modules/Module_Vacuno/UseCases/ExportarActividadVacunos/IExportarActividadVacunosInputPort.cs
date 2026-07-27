using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Common.Gateway.Reports;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarActividadVacunos
{
    public interface IExportarActividadVacunosInputPort
        : IRequestHandler<ExportarActividadVacunosQuery , GeneratedReportDocument>
    {
    }
}