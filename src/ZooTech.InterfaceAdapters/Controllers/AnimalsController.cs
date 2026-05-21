using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Animals.UseCases.DeleteAnimal;
using ZooTech.InterfaceAdapters.DTOs.Requests;
using ZooTech.InterfaceAdapters.Presenters;

namespace ZooTech.InterfaceAdapters.Controllers;

[ApiController]
[Route("api/animals")]
[ApiExplorerSettings(GroupName = "public")]
public sealed class AnimalsController : ControllerBase
{
    private const string DeletedByHeaderName = "X-User-Id";
    private readonly IDeleteAnimalInputPort deleteAnimalInputPort;

    public AnimalsController(IDeleteAnimalInputPort deleteAnimalInputPort)
    {
        this.deleteAnimalInputPort = deleteAnimalInputPort;
    }

    [HttpDelete("{id:long}")]
    [Tags("Animals")]
    public async Task<IActionResult> Delete(
        long id,
        [FromBody] DeleteAnimalRequest request,
        CancellationToken cancellationToken)
    {
        var presenter = new DeleteAnimalPresenter();
        var command = new DeleteAnimalCommand(
            id,
            request.MotivoEliminacion,
            GetDeletedByFromHeader());

        await deleteAnimalInputPort.Handle(command, presenter, cancellationToken);

        return presenter.Result;
    }

    private long? GetDeletedByFromHeader()
    {
        if (!Request.Headers.TryGetValue(DeletedByHeaderName, out var value))
        {
            return null;
        }

        return long.TryParse(value.ToString(), out var deletedBy)
            ? deletedBy
            : -1;
    }
}
