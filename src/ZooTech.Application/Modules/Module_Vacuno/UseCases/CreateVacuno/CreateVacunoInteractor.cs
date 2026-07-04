using FluentValidation;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_Vacuno.Common;
using ZooTech.Domain.Module_Vacuno.Entities;
using ZooTech.Domain.Module_Vacuno.Interfaces;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.CreateVacuno;

public sealed class CreateVacunoInteractor : ICreateVacunoInputPort
{
    private readonly IVacunoRepository _repository;
    private readonly IValidator<CreateVacunoCommand> _validator;

    public CreateVacunoInteractor(IVacunoRepository repository, IValidator<CreateVacunoCommand> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<CreateVacunoOutput> HandleAsync(CreateVacunoCommand command, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        if (await _repository.ExistsCodigoAsync(command.Codigo, cancellationToken))
            throw new ConflictException("Ya existe un vacuno con el mismo código.");

        Vacuno vacuno;
        try
        {
            vacuno = Vacuno.CreateNew(
                command.Codigo,
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

        var saved = await _repository.AddAsync(vacuno, cancellationToken);
        return new CreateVacunoOutput(VacunoAppMapper.ToOutput(saved));
    }
}
