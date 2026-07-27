using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Common.Models;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionOptions;

public interface IGetFecundacionOptionsInputPort
    : IRequestHandler<EmptyCommandQuery , GetFecundacionOptionsOutput>
{
}
