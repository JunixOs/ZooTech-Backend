namespace ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;

public sealed class ReportAnimalListInteractor : IReportAnimalListInputPort
{
    private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    private const string PdfContentType = "application/pdf";
    private readonly IAnimalReportRepository repository;
    private readonly IAnimalReportExcelService excelService;
    private readonly IAnimalReportPdfService pdfService;
    private readonly ReportAnimalListValidator validator;

    public ReportAnimalListInteractor(
        IAnimalReportRepository repository,
        IAnimalReportExcelService excelService,
        IAnimalReportPdfService pdfService,
        ReportAnimalListValidator validator)
    {
        this.repository = repository;
        this.excelService = excelService;
        this.pdfService = pdfService;
        this.validator = validator;
    }

    public async Task Handle(
        ReportAnimalListCommand command,
        IReportAnimalListOutputPort outputPort,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var filter = validator.ValidateAndNormalize(command);
            var items = await repository.GetAnimalListAsync(filter, cancellationToken);

            if (items is null)
            {
                items = [];
            }

            var output = new ReportAnimalListOutput(
                filter.FechaInicio,
                filter.FechaFin,
                filter.Keyword,
                filter.RazaCode,
                filter.SexoCode,
                filter.TipoAdquisicionCode,
                filter.GranjaId,
                filter.EstadoCode,
                items);

            if (command.ExportExcel)
            {
                var content = excelService.GenerateAnimalListExcel(output);
                outputPort.PresentExcel(new ReportAnimalListExcelOutput(
                    $"reporte-vacunos-{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx",
                    ExcelContentType,
                    content));
                return;
            }

            if (command.ExportPdf)
            {
                var content = pdfService.GenerateAnimalListPdf(output);
                outputPort.PresentPdf(new ReportAnimalListPdfOutput(
                    $"reporte-vacunos-{DateTime.UtcNow:yyyyMMddHHmmss}.pdf",
                    PdfContentType,
                    content));
                return;
            }

            outputPort.PresentList(output);
        }
        catch (ReportAnimalListValidationException exception)
        {
            outputPort.PresentValidationError(exception);
        }
        catch (AnimalReportNotFoundException exception)
        {
            outputPort.PresentNotFound(exception);
        }
        catch (Exception exception)
        {
            outputPort.PresentUnexpectedError(new AnimalReportGenerationException(exception));
        }
    }
}
