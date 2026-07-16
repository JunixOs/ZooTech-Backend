using ZooTech.Application.Modules.Module_Vacuno.Exceptions;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;
using ZooTech.Domain.Module_Celo.Interfaces;
using ZooTech.Domain.Module_Sanidad.Interfaces;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;
using ZooTech.Domain.Shared.Enums;
using ZooTech.Application.Common.Models;
using ZooTech.Domain.Shared.Interfaces;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.DeleteVacuno;

public sealed class DeleteVacunoInteractor : IDeleteVacunoInputPort
{
    private readonly IGanaderiaUnitOfWork _unitOfWork;
   

    public DeleteVacunoInteractor(
        IGanaderiaUnitOfWork unitOfWork;
    )
    {
        _unitOfWork = unitOfWork;
    }

    var repository = _unitOfWork.Vacunos;

    public async Task<EmptyOutput> HandleAsync(DeleteVacunoCommand command, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new VacunoNotFoundException($"No existe el vacuno con el ID {command.Id}.");
        try
        {
            existing.SoftDelete(command.MotivoEliminacion, null, DateTime.UtcNow);
        }
        catch (ArgumentException ex)
        {
            throw new VacunoException(
                ErrorType.Validation,
                "BAD_REQUEST", 
                message: ex.Message
            );
        }

        _ = await repository.UpdateAsync(existing, null, null, cancellationToken);

        return EmptyOutput.Value;
    }
}
