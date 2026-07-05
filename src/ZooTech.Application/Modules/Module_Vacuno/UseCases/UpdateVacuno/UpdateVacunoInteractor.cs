using FluentValidation;
using ZooTech.Application.Modules.Module_Vacuno.Exceptions;
using ZooTech.Application.Modules.Module_Vacuno.Common;
using ZooTech.Domain.Module_Vacuno.Interfaces;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.UpdateVacuno;

public sealed class UpdateVacunoInteractor : IUpdateVacunoInputPort
{
    private readonly IVacunoRepository _repository;
    private readonly IValidator<UpdateVacunoCommand> _validator;

    public UpdateVacunoInteractor(IVacunoRepository repository, IValidator<UpdateVacunoCommand> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<UpdateVacunoOutput> HandleAsync(long id, UpdateVacunoCommand command, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var existing = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new VacunoNotFoundException($"No existe el vacuno con el ID {id}.");

        // Validaciones de inmutabilidad (PDF pág 8: "No se permite editar: codigo, fechaNacimiento, adquisicionPor")
        if (existing.FechaNacimiento != command.FechaNacimiento)
        {
            throw new ImmutableFieldException("No se permite editar la fecha de nacimiento.");
        }

        if (existing.TipoAdquisicionCode != command.TipoAdquisicionCode)
        {
            throw new ImmutableFieldException("No se permite editar el tipo de adquisición.");
        }

        try
        {
            existing.Update(
                command.Nombre,
                command.FechaNacimiento,
                command.TipoAdquisicionCode,
                command.RazaCode,
                command.ColorCode,
                command.SexoCode,
                command.PadreId,
                command.MadreId,
                command.GranjaId,
                command.Observaciones,
                null,
                DateTime.UtcNow);
        }
        catch (ArgumentException ex)
        {
            throw new VacunoException(ex.Message, "BAD_REQUEST", 400);
        }

        var updated = await _repository.UpdateAsync(existing, command.PrecioCompra, command.AptoPara, cancellationToken);
        return new UpdateVacunoOutput(VacunoAppMapper.ToOutput(updated));
    }
}
