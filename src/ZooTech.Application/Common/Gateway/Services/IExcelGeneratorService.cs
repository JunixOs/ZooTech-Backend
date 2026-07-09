namespace ZooTech.Application.Common.Gateway.Services;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosExcel;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesExcel;
public interface IExcelGeneratorService 
{
    byte[] GenerateOrdeniosReport(GenerateOrdeniosExcelDocument document);
    byte[] GenerateTriajesReport(GenerateTriajesExcelDocument document);

}
