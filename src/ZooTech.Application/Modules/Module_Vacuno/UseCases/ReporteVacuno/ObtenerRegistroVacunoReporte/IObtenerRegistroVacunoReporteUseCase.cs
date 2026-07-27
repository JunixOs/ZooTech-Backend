using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte;

public interface IObtenerRegistroVacunoReporteUseCase
    : IRequestHandler<ObtenerRegistroVacunoReporteQuery , RegistroVacunoReporteResponse>
{
}
