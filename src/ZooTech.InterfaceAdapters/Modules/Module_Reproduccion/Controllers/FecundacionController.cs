using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Module_Reproduccion.UseCases.ConfirmarFecundacion;
using ZooTech.Application.Modules.Module_Reproduccion.UseCases.RegistrarFecundacion;
using ZooTech.InterfaceAdapters.Modules.Module_Reproduccion.DTOs.Fecundacion.Requests;

namespace ZooTech.InterfaceAdapters.Modules.Module_Reproduccion.Controllers;

[ApiController]
[Route("api/v1/fecundaciones")]
[Authorize]
public sealed class FecundacionController : ControllerBase
{
    private readonly IRegistrarFecundacionInputPort _registrarFecundacionInputPort;
    private readonly IConfirmarFecundacionInputPort _confirmarFecundacionInputPort;

    public FecundacionController(
        IRegistrarFecundacionInputPort registrarFecundacionInputPort,
        IConfirmarFecundacionInputPort confirmarFecundacionInputPort)
    {
        _registrarFecundacionInputPort = registrarFecundacionInputPort;
        _confirmarFecundacionInputPort = confirmarFecundacionInputPort;
    }

    [HttpPost]
    public async Task<IActionResult> RegistrarAsync(
        [FromBody] RegistrarFecundacionRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new RegistrarFecundacionCommand(
            request.TipoFecundacionCode,
            request.VacunoReceptorId,
            request.VacunoDonanteId,
            request.NombreMachoExterno,
            request.FechaProcedimiento,
            request.ResponsableId,
            request.CodigoSemen,
            request.CodigoEmbrion,
            request.Observaciones,
            1 // Default UserID until auth is fully integrated
        );

        var output = await _registrarFecundacionInputPort.HandleAsync(command, cancellationToken);

        return Created($"/api/v1/fecundaciones/{output.Id}", output);
    }

    [HttpPatch("{id:long}/resultado")]
    public async Task<IActionResult> ConfirmarResultadoAsync(
        [FromRoute] long id,
        [FromBody] ConfirmarFecundacionRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new ConfirmarFecundacionCommand(
            id,
            request.NuevoResultadoCode,
            1 // Default UserID
        );

        var output = await _confirmarFecundacionInputPort.HandleAsync(command, cancellationToken);

        return Ok(output);
    }
}
