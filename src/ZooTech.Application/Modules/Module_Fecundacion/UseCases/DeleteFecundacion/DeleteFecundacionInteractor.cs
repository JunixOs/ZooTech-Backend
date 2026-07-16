using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Fecundacion.Common;
using ZooTech.Application.Modules.Module_Fecundacion.Exceptions;
using ZooTech.Domain.Shared.Interfaces;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.DeleteFecundacion;

public sealed class DeleteFecundacionInteractor : IDeleteFecundacionInputPort
{
    private readonly IGanaderiaUnitOfWork _unitOfWork;
    private readonly IAppCacheService _cache;

    public DeleteFecundacionInteractor(
        IGanaderiaUnitOfWork unitOfWork,
        IAppCacheService cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<EmptyOutput> HandleAsync(DeleteFecundacionCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Razon))
        {
            throw new ArgumentException("Falta razón de eliminación o datos inválidos.");
        }

        var repository = _unitOfWork.Fecundaciones;
        var existing = await repository.GetForEditAsync(command.Id, cancellationToken);
        if (existing is null)
        {
            throw new FecundacionNotFoundException();
        }

        // Validar si tiene crías vinculadas en trazabilidad
        var hasCria = await repository.HasCriaAsync(command.Id, cancellationToken);
        if (hasCria)
        {
            throw new FecundacionHasDependenciesException();
        }

        _ = await _unitOfWork.ExecuteInTransactionAsync(
            async ct =>
            {
                await repository.DeleteAsync(command.Id, command.Razon, ct);
                return EmptyOutput.Value;
            },
            cancellationToken);
        await _cache.RemoveByPrefixAsync(FecundacionCacheKeys.ListarPrefix);

        return EmptyOutput.Value;
    }
}
