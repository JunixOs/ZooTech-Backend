using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Mappers;
using Microsoft.AspNetCore.Http;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Controllers;

[ApiController]
[Route("v1/vacunos")]
public sealed class VacunoController : ControllerBase
{
    private readonly IMediator _mediator;

    public VacunoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(RegistrarVacunoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegistrarVacuno(
        [FromForm] RegistrarVacunoRequest request,
        CancellationToken cancellationToken)
    {
        var command = VacunoMapper.ToCommand(request);
        var result = await _mediator.Send(command, cancellationToken);

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var response = VacunoMapper.ToResponse(result, baseUrl);

        return CreatedAtAction(
            actionName: nameof(ObtenerVacuno),
            routeValues: new { vacunoId = response.Id },
            value: response
        );
    }

    [HttpGet("{vacunoId:int}")]
    public IActionResult ObtenerVacuno(int vacunoId)
        => Ok(); // Placeholder para el Location header
}