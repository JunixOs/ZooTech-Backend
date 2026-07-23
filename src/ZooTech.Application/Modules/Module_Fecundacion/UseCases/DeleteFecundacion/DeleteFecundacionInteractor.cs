using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Fecundacion.Common;
using ZooTech.Application.Modules.Module_Fecundacion.Exceptions;
using ZooTech.Domain.Shared.Interfaces;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.DeleteFecundacion;

public sealed class DeleteFecundacionInteractor : IDeleteFecundacionInputPort
{
    private readonly IGanaderiaUnitOfWork _unitOfWork;

    public DeleteFecundacionInteractor(IGanaderiaUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<EmptyOutput> HandleAsync(
        DeleteFecundacionCommand command,
        CancellationToken cancellationToken)
    {
        _ = await _unitOfWork.ExecuteInTransactionAsync(
            async ct =>
            {
                var repository = _unitOfWork.Fecundaciones;
                var existing = await repository.GetForEditAsync(command.Id, ct);
                if (existing is null)
                {
                    throw new FecundacionNotFoundException();
                }

                if (await repository.HasCriaAsync(command.Id, ct))
                {
                    throw new FecundacionHasDependenciesException();
                }

                await repository.DeleteAsync(command.Id, command.Razon, ct);
                return EmptyOutput.Value;
            },
            cancellationToken);

        return EmptyOutput.Value;
    }
}
