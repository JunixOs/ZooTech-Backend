using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosExcel;

namespace ZooTech.Application.Common.Gateway.Services;

public interface IOrdeniosComparationExcelGeneratorService
{
    byte[] GenerateOrdeniosReport(GenerateOrdeniosExcelDocument document);
}
