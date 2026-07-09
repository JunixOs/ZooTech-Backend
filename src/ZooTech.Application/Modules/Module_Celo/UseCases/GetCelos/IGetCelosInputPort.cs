using ZooTech.Application.Common.Models;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetCelos;

public interface IGetCelosInputPort
{
    Task<GetCelosOutput> Handle(EmptyCommand emptyCommand, CancellationToken cancellationToken = default);
}
    