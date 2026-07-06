using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Animals.UseCases.DeleteAnimal;
using ZooTech.InterfaceAdapters.DTOs.Responses;
using ZooTech.InterfaceAdapters.Mappers;

namespace ZooTech.InterfaceAdapters.Presenters;

public sealed class DeleteAnimalPresenter : IDeleteAnimalOutputPort
{
    public IActionResult Result { get; private set; } =
        new StatusCodeResult(StatusCodes.Status500InternalServerError);

    public void PresentSuccess(DeleteAnimalOutput output)
    {
        Result = new OkObjectResult(AnimalMapper.ToDeleteResponse(output));
    }

    public void PresentNotFound(AnimalNotFoundException exception)
    {
        Result = new NotFoundObjectResult(ErrorResponse.Create(
            "ANIMAL_NOT_FOUND",
            exception.Message));
    }

    public void PresentValidationError(AnimalDeleteValidationException exception)
    {
        var details = exception.Errors.Select(error => new ErrorDetail
        {
            Field = error.Key,
            Message = error.Value
        });

        Result = new BadRequestObjectResult(ErrorResponse.Create(
            "ANIMAL_DELETE_VALIDATION_ERROR",
            exception.Message,
            details));
    }

    public void PresentConflict(AnimalHasDependenciesException exception)
    {
        Result = new ConflictObjectResult(ErrorResponse.Create(
            "ANIMAL_HAS_DEPENDENCIES",
            exception.Message));
    }

    public void PresentDuplicateConflict(AnimalDuplicateException exception)
    {
        Result = new ConflictObjectResult(ErrorResponse.Create(
            "ANIMAL_DUPLICATE_CONFLICT",
            exception.Message));
    }
}
