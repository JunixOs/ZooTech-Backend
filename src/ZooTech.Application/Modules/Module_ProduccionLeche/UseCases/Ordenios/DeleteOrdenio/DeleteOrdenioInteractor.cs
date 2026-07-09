using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Models;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.DeleteOrdenio;

public sealed class DeleteOrdenioInteractor : IDeleteOrdenioInputPort
{
    private readonly IOrdenioUnitOfWork _unitOfWork;

    public DeleteOrdenioInteractor(IOrdenioUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<EmptyOutput> Handle(DeleteOrdenioCommand command, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository;

        var existing = await repository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new NotFoundException(
                ScopeName.Application,
                ModuleName.Produccion_Leche,
                "No se encontró el ordeño solicitado."
            );

        try
        {
            existing.SoftDelete(command.MotivoEliminacion, null, DateTime.UtcNow);
        }
        catch (ArgumentException ex)
        {
            throw new ConflictException(
                ScopeName.Application,
                ModuleName.Produccion_Leche,
                message: ex.Message
            );
        }

        _ = await _unitOfWork.ExecuteInTransactionAsync(
            ct => repository.UpdateAsync(existing, ct),
            cancellationToken);

        return EmptyOutput.Value;
    }
}
