using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Common.Models;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllVacunosSanidad;

public interface IGetAllVacunosSanidadInputPort
    : IRequestHandler<EmptyCommandQuery , GetAllVacunosSanidadOutput>
{
}
