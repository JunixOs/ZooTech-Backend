using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialGeneral;

public interface IGetHistorialGeneralInputPort
    : IRequestHandler<GetHistorialGeneralQuery , GetHistorialGeneralOutput>
{
}
