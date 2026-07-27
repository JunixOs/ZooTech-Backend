using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.UpdateFecundacion;

public interface IUpdateFecundacionInputPort
    : IRequestHandler<UpdateFecundacionCommand , UpdateFecundacionOutput>
{
}
