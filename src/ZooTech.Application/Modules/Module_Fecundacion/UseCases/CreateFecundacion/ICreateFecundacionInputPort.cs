using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.CreateFecundacion;

public interface ICreateFecundacionInputPort
    : IRequestHandler<CreateFecundacionCommand , CreateFecundacionOutput>
{
}
