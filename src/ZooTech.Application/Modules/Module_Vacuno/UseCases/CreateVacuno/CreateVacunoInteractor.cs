using FluentValidation;
using ZooTech.Application.Modules.Module_Vacuno.Exceptions;
using ZooTech.Application.Modules.Module_Vacuno.Common;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;
using ZooTech.Domain.Shared.Enums;
using ZooTech.Domain.Shared.Interfaces;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.CreateVacuno;

public sealed class CreateVacunoInteractor : ICreateVacunoInputPort
{
    private readonly IGanaderiaUnitOfWork _unitOfWork;

    public CreateVacunoInteractor(
        IGanaderiaUnitOfWork unitOfWork
    )
    {
        _unitOfWork = unitOfWork;
    }

    var repository =_unitOfWork.Vacunos;

    public async Task<CreateVacunoOutput> HandleAsync(CreateVacunoCommand command, CancellationToken cancellationToken)
    {
        if (await repository.ExistsCodigoAsync(command.Codigo, cancellationToken))
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

         var saved = await _unitOfWork.ExecuteInTransactionAsync(
            ct => repository.AddAsync(vacuno,command.PrecioCompra, command.AptoPara, ct),
            cancellationToken,
            async (_, ct) => await repository.GetByCodigoAsync(command.Codigo, ct)
                ?? throw new InvalidOperationException("No se pudo recuperar el ordeño creado."));
        return new CreateVacunoOutput(VacunoAppMapper.ToOutput(saved));
    }
}
