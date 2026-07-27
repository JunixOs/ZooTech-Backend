using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ListarVacunosReporte;

public interface IListarVacunosReporteUseCase
    : IRequestHandler<ListarVacunosReporteQuery , ListarVacunosReporteResponse>
{
}
