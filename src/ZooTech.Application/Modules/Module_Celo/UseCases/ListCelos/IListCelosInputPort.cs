using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.ListCelos;

public interface IListCelosInputPort
    : IRequestHandler<ListCelosQuery , ListCelosOutput>
{
}
