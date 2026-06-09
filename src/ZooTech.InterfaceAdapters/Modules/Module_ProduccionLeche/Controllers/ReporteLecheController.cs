using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Reportes.GenerateReporteExcel;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Reportes.GetReporteDiario;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Reportes.GenerateReportePdf;

namespace ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.Controllers;


[ApiController]
[ApiVersion("1.0")]
[Route("api/v1/{version:apiVersion}/produccion-leche/reporte")]
public class ReporteLecheController : ControllerBase
{
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            status = "Api funcionando de manera correcta",
            model = "reporte produccion leche"
        });
    }

    [HttpGet("diario")]
    public async Task<IActionResult> GetReporteDiario(
        [FromQuery] DateTime? fechaDesde,
        [FromQuery] DateTime? fechaHasta,
        [FromQuery] long? vacunoId,
        [FromServices] IGetReporteDiarioInputPort inputPort,
        CancellationToken cancellationToken)
    {
        var query = new ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Reportes.GetReporteDiario.GetReporteDiarioQuery(fechaDesde, fechaHasta, vacunoId);
        var output = await inputPort.HandleAsync(query, cancellationToken);
        var response = ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.Mappers.ProduccionLecheMapper.ToResponse(output);
        return Ok(response);
    }

    [HttpGet("diario/excel")]
    public async Task<IActionResult> GenerateReporteExcel(
        [FromQuery] DateTime? fechaDesde,
        [FromQuery] DateTime? fechaHasta,
        [FromQuery] long? vacunoId,
        [FromServices] IGenerateReporteExcelInputPort inputPort,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = new GenerateReporteExcelQuery(fechaDesde, fechaHasta, vacunoId);
            var output = await inputPort.HandleAsync(query, cancellationToken);

            return File(
                output.ExcelBytes,
                output.ContentType,
                output.FileName);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error al generar el Excel", details = ex.Message });
        }
    }

    [HttpGet("diario/pdf")]
    public async Task<IActionResult> GenerateReportePdfProfesional(
        [FromQuery] DateTime? fechaDesde,
        [FromQuery] DateTime? fechaHasta,
        [FromQuery] long? vacunoId,
        [FromServices] IGenerateReportePdfInputPort inputPort,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = new GenerateReportePdfQuery(fechaDesde, fechaHasta, vacunoId);
            var output = await inputPort.HandleAsync(query, cancellationToken);

            return File(
                output.PdfBytes,
                output.ContentType,
                output.FileName);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error al generar el PDF", details = ex.Message });
        }
    }
}
