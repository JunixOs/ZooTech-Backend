using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;

public interface IListarVacunosInputPort
    : IRequestHandler<ListarVacunosQuery , ListarVacunosOutput>
{
}
