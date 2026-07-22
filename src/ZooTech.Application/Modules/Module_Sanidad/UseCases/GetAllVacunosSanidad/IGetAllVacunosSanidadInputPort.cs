using ZooTech.Application.Common.Models;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllVacunosSanidad;

public interface IGetAllVacunosSanidadInputPort
{
    Task<GetAllVacunosSanidadOutput> Handle(EmptyCommand emptyCommand, CancellationToken cancellationToken = default);
}
