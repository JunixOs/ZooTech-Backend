using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.ListarFecundacion;

public interface IListarFecundacionInputPort
    : IRequestHandler<ListarFecundacionQuery , ListarFecundacionOutput>
{
}