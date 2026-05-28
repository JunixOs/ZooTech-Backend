namespace ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;

public sealed class ReportAnimalListInteractor : IReportAnimalListInputPort
{
    private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    private readonly IAnimalReportRepository repository;
    private readonly IAnimalReportExcelService excelService;
    private readonly ReportAnimalListValidator validator;

    public ReportAnimalListInteractor(
        IAnimalReportRepository repository,
        IAnimalReportExcelService excelService,
        ReportAnimalListValidator validator)
    {
        this.repository = repository;
        this.excelService = excelService;
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

            if (items.Count == 0)
            {
                throw new AnimalReportNotFoundException();
            }

            var output = new ReportAnimalListOutput(
                filter.FechaInicio,
                filter.FechaFin,
                filter.Keyword,
                items);

            if (!command.ExportExcel)
            {
                outputPort.PresentList(output);
                return;
            }

            var content = excelService.GenerateAnimalListExcel(output);
            outputPort.PresentExcel(new ReportAnimalListExcelOutput(
                $"reporte-vacunos-{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx",
                ExcelContentType,
                content));
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
