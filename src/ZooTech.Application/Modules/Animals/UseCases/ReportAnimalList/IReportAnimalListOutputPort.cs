namespace ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;

public interface IReportAnimalListOutputPort
{
    void PresentList(ReportAnimalListOutput output);

    void PresentExcel(ReportAnimalListExcelOutput output);

    void PresentPdf(ReportAnimalListPdfOutput output);

    void PresentValidationError(ReportAnimalListValidationException exception);

    void PresentNotFound(AnimalReportNotFoundException exception);

    void PresentUnexpectedError(AnimalReportGenerationException exception);
}
