using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesExcel;

public interface IGenerateTriajesExcelInputPort
    : IRequestHandler<GenerateTriajesExcelQuery , GenerateTriajesExcelOutput>
{
}
