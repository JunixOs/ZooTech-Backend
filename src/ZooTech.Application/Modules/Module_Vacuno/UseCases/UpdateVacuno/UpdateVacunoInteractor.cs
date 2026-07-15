using ZooTech.Application.Modules.Module_Vacuno.Exceptions;
using ZooTech.Application.Modules.Module_Vacuno.Common;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.UpdateVacuno;

public sealed class UpdateVacunoInteractor : IUpdateVacunoInputPort
{
    private readonly IVacunoRepository _repository;

    public UpdateVacunoInteractor(IVacunoRepository repository)
    {
        _repository = repository;
    }

    public async Task<UpdateVacunoOutput> HandleAsync(UpdateVacunoCommand command, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new VacunoNotFoundException($"No existe el vacuno con el ID {command.Id}.");

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
            throw new VacunoException(
                ErrorType.Validation,
                "BAD_REQUEST", 
                message: ex.Message
            );
        }

        var updated = await _repository.UpdateAsync(existing, command.PrecioCompra, command.AptoPara, cancellationToken);
        return new UpdateVacunoOutput(VacunoAppMapper.ToOutput(updated));
    }
}
