using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;
using ZooTech.InterfaceAdapters.DTOs.Responses;
using ZooTech.InterfaceAdapters.Mappers;

namespace ZooTech.InterfaceAdapters.Presenters;

public sealed class ReportAnimalListPresenter : IReportAnimalListOutputPort
{
    public IActionResult Result { get; private set; } =
        new StatusCodeResult(StatusCodes.Status500InternalServerError);

    public void PresentList(ReportAnimalListOutput output)
    {
        Result = new OkObjectResult(AnimalMapper.ToReportAnimalListResponse(output));
    }

    public void PresentExcel(ReportAnimalListExcelOutput output)
    {
        Result = new FileContentResult(output.Content, output.ContentType)
        {
            FileDownloadName = output.FileName
        };
    }

    public void PresentPdf(ReportAnimalListPdfOutput output)
    {
        Result = new FileContentResult(output.Content, output.ContentType)
        {
            FileDownloadName = output.FileName
        };
    }

    public void PresentValidationError(ReportAnimalListValidationException exception)
    {
        var details = exception.Errors.Select(error => new ErrorDetail
        {
            Field = error.Key,
            Message = error.Value
        });

        Result = new BadRequestObjectResult(ErrorResponse.Create(
            "ANIMAL_REPORT_VALIDATION_ERROR",
            exception.Message,
            details));
    }

    public void PresentNotFound(AnimalReportNotFoundException exception)
    {
        Result = new NotFoundObjectResult(ErrorResponse.Create(
            "ANIMAL_REPORT_NOT_FOUND",
            exception.Message));
    }

    public void PresentUnexpectedError(AnimalReportGenerationException exception)
    {
        var details = new[]
        {
            new ErrorDetail
            {
                Field = "exception.Message",
                Message = exception.OriginalExceptionMessage
            },
            new ErrorDetail
            {
                Field = "exception.InnerException.Message",
                Message = exception.OriginalInnerExceptionMessage ?? string.Empty
            },
            new ErrorDetail
            {
                Field = "exception.GetType().Name",
                Message = exception.OriginalExceptionTypeName
            }
        };

        Result = new ObjectResult(ErrorResponse.Create(
            "ANIMAL_REPORT_GENERATION_ERROR",
            exception.Message,
            details))
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
    }
}
