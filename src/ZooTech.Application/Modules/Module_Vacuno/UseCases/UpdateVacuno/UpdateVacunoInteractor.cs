using FluentValidation;
using ZooTech.Application.Common.Exceptions;
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
            ?? throw new NotFoundException("No se encontró el vacuno solicitado.");

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
            throw new ConflictException(ex.Message);
        }

        var updated = await _repository.UpdateAsync(existing, cancellationToken);
        return new UpdateVacunoOutput(VacunoAppMapper.ToOutput(updated));
    }
}
