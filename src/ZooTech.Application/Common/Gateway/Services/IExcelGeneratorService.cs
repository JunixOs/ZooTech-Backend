namespace ZooTech.Application.Common.Gateway.Services;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosExcel;

public interface IExcelGeneratorService 
{
    byte[] GenerateOrdeniosReport(GenerateOrdeniosExcelDocument document);


}
