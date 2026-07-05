namespace ZooTech.Application.Modules.Animals.UseCases.DeleteAnimal;

public sealed class AnimalDeleteValidationException : Exception
{
    public AnimalDeleteValidationException(IReadOnlyDictionary<string, string> errors)
        : base("Los datos enviados para eliminar el vacuno no son validos.")
    {
        Errors = errors;
    }

    public IReadOnlyDictionary<string, string> Errors { get; }
}
