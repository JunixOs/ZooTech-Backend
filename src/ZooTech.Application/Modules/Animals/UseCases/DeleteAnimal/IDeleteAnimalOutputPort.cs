namespace ZooTech.Application.Modules.Animals.UseCases.DeleteAnimal;

public interface IDeleteAnimalOutputPort
{
    void PresentSuccess(DeleteAnimalOutput output);

    void PresentNotFound(AnimalNotFoundException exception);

    void PresentValidationError(AnimalDeleteValidationException exception);

    void PresentConflict(AnimalHasDependenciesException exception);

    void PresentDuplicateConflict(AnimalDuplicateException exception);
}
