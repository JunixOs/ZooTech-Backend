namespace ZooTech.Application.Modules.Animals.UseCases.DeleteAnimal;

public sealed class DeleteAnimalValidator
{
    private const int MotivoMaxLength = 250;

    public void Validate(DeleteAnimalCommand command)
    {
        Dictionary<string, string> errors = [];

        if (command.Id <= 0)
        {
            errors["id"] = "El ID del vacuno debe ser mayor a cero.";
        }

        if (string.IsNullOrWhiteSpace(command.MotivoEliminacion))
        {
            errors["motivoEliminacion"] = "El motivo de eliminacion es obligatorio.";
        }
        else if (command.MotivoEliminacion.Trim().Length > MotivoMaxLength)
        {
            errors["motivoEliminacion"] = $"El motivo de eliminacion no debe superar {MotivoMaxLength} caracteres.";
        }

        if (command.EliminadoPor.HasValue && command.EliminadoPor <= 0)
        {
            errors["eliminadoPor"] = "El usuario que elimina debe ser mayor a cero.";
        }

        if (errors.Count > 0)
        {
            throw new AnimalDeleteValidationException(errors);
        }
    }
}
