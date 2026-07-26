
using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosExcel;

public interface IGetOrdeniosExcelInputPort
    : IRequestHandler<GenerateOrdeniosComparationExcelQuery , GenerateOrdeniosExcelOutput>
{
}


