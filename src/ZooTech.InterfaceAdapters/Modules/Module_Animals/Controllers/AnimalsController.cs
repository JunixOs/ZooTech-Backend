using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Module_Animals.UseCases.ListAnimals;
using ZooTech.Domain.Enums;
using ZooTech.InterfaceAdapters.Modules.Module_Animals.Presenters;

namespace ZooTech.InterfaceAdapters.Modules.Module_Animals.Controllers;

[ApiController]
[Route("v1/vacunos")]
public class AnimalsController : ControllerBase
{
    private readonly IListAnimalsInputPort _inputPort;
    private readonly ListAnimalsPresenter _presenter;

    public AnimalsController(
        IListAnimalsInputPort inputPort,
        ListAnimalsPresenter presenter)
    {
        _inputPort = inputPort;
        _presenter = presenter;
    }

    [HttpGet]
    public async Task<IActionResult> List(
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

        var command = new ListAnimalsCommand
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
