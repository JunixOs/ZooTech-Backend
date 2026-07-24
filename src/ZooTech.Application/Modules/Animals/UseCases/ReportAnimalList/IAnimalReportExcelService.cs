namespace ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;

public interface IAnimalReportExcelService
{
    byte[] GenerateAnimalListExcel(ReportAnimalListOutput output);
}
