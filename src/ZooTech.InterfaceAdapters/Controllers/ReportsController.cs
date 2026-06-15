using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;
using ZooTech.InterfaceAdapters.DTOs.Requests;
using ZooTech.InterfaceAdapters.Presenters;

namespace ZooTech.InterfaceAdapters.Controllers;

[ApiController]
[Route("api/reports/animals")]
[ApiExplorerSettings(GroupName = "public")]
public sealed class ReportsController : ControllerBase
{
    private readonly IReportAnimalListInputPort reportAnimalListInputPort;

    public ReportsController(IReportAnimalListInputPort reportAnimalListInputPort)
    {
        this.reportAnimalListInputPort = reportAnimalListInputPort;
    }

    [HttpGet]
    [Tags("Reports")]
    public async Task<IActionResult> GetAnimals(
        [FromQuery] ReportAnimalListRequest request,
        CancellationToken cancellationToken)
    {
        return await HandleReportAsync(request, exportExcel: false, exportPdf: false, cancellationToken);
    }

    [HttpGet("excel")]
    [Tags("Reports")]
    public async Task<IActionResult> DownloadAnimalsExcel(
        [FromQuery] ReportAnimalListRequest request,
        CancellationToken cancellationToken)
    {
        return await HandleReportAsync(request, exportExcel: true, exportPdf: false, cancellationToken);
    }

    [HttpGet("pdf")]
    [Tags("Reports")]
    public async Task<IActionResult> DownloadAnimalsPdf(
        [FromQuery] ReportAnimalListRequest request,
        CancellationToken cancellationToken)
    {
        return await HandleReportAsync(request, exportExcel: false, exportPdf: true, cancellationToken);
    }

    private async Task<IActionResult> HandleReportAsync(
        ReportAnimalListRequest request,
        bool exportExcel,
        bool exportPdf,
        CancellationToken cancellationToken)
    {
        var presenter = new ReportAnimalListPresenter();
        var command = new ReportAnimalListCommand(
            request.FechaInicio,
            request.FechaFin,
            request.Keyword,
            exportExcel,
            exportPdf);

        await reportAnimalListInputPort.Handle(command, presenter, cancellationToken);

        return presenter.Result;
    }
}
