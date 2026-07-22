using ZooTech.Application.Common.Models;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTipoPesos;

public interface IGetAllTipoPesosInputPort
{
    Task<GetAllTipoPesosOutput> Handle(EmptyCommand emptyCommand, CancellationToken cancellationToken = default);
}
