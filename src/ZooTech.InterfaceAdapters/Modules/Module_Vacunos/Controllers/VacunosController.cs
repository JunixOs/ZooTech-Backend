using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Module_Vacunos.UseCases.ListarVacunos;
using ZooTech.Domain.Enums;
using ZooTech.InterfaceAdapters.Modules.Module_Vacunos.Presenters;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacunos.Controllers;

[ApiController]
[Route("v1/vacunos")]
[ApiExplorerSettings(GroupName = "public")]
public class VacunosController : ControllerBase
{
    private readonly IListarVacunosInputPort _inputPort;
    private readonly ListarVacunosPresenter _presenter;

    public VacunosController(
        IListarVacunosInputPort inputPort,
        ListarVacunosPresenter presenter)
    {
        _inputPort = inputPort;
        _presenter = presenter;
    }

    [HttpGet]
    [Tags("Vacunos")]
    public async Task<IActionResult> Listar(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20,
        [FromQuery] DateTime? fechaDesde = null,
        [FromQuery] DateTime? fechaHasta = null,
        [FromQuery] string? estado = null,
        [FromQuery] string? q = null)
    {
        EstadoAnimal? estadoEnum = null;
        if (!string.IsNullOrEmpty(estado))
        {
            if (Enum.TryParse<EstadoAnimal>(estado, true, out var parsed))
            {
                estadoEnum = parsed;
            }
            else
            {
                return BadRequest(new
                {
                    error = new
                    {
                        code = "VALIDATION_ERROR",
                        message = "Los datos enviados no son válidos.",
                        details = new[]
                        {
                            new { field = "estado", message = "Valores permitidos: vivo, muerto." }
                        }
                    }
                });
            }
        }

        var command = new ListarVacunosCommand
        {
            Page = page,
            Limit = limit,
            FechaDesde = fechaDesde,
            FechaHasta = fechaHasta,
            Estado = estadoEnum,
            Q = q
        };

        await _inputPort.Handle(command);

        return StatusCode(_presenter.StatusCode, _presenter.Response);
    }
}
