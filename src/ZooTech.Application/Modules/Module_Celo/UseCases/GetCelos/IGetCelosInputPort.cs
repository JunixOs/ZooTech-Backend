using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Common.Models;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetCelos;

public interface IGetCelosInputPort
    : IRequestHandler<EmptyCommand , GetCelosOutput>
{
}
    