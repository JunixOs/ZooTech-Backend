using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Common.Models;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.DeleteFecundacion;

public interface IDeleteFecundacionInputPort
    : IRequestHandler<DeleteFecundacionCommand , EmptyOutput>
{
}
