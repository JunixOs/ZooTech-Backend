namespace ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;

public interface IAnimalReportPdfService
{
    byte[] GenerateAnimalListPdf(ReportAnimalListOutput output);
}
