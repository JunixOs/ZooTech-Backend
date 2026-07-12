using FluentValidation;
using ZooTech.Application.Modules.Module_Vacuno.Exceptions;
using ZooTech.Application.Modules.Module_Vacuno.Common;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.CreateVacuno;

public sealed class CreateVacunoInteractor : ICreateVacunoInputPort
{
    private readonly IVacunoRepository _repository;

    public CreateVacunoInteractor(
        IVacunoRepository repository
    )
    {
        _repository = repository;
    }

    public async Task<CreateVacunoOutput> HandleAsync(CreateVacunoCommand command, CancellationToken cancellationToken)
    {
        if (await _repository.ExistsCodigoAsync(command.Codigo, cancellationToken))
            throw new VacunoAlreadyExistsException("Ya existe un vacuno con ese código o datos repetidos.");

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
            throw new VacunoException(
                ErrorType.Validation,
                "BAD_REQUEST", 
                message: ex.Message
            );
        }

        var saved = await _repository.AddAsync(vacuno, command.PrecioCompra, command.AptoPara, cancellationToken);
        return new CreateVacunoOutput(VacunoAppMapper.ToOutput(saved));
    }
}
