using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Common.Models;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTipoPesos;

public interface IGetAllTipoPesosInputPort
    : IRequestHandler<EmptyCommandQuery , GetAllTipoPesosOutput>
{
}
