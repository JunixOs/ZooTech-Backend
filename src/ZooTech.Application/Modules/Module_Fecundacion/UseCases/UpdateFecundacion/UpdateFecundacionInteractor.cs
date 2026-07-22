using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Application.Modules.Module_Fecundacion.Common;
using ZooTech.Application.Modules.Module_Fecundacion.Exceptions;
using ZooTech.Domain.Ganaderia.Module_Fecundacion.Interfaces;
using ZooTech.Domain.Shared.Interfaces;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.UpdateFecundacion;

public sealed class UpdateFecundacionInteractor : IUpdateFecundacionInputPort
{
    private readonly IGanaderiaUnitOfWork _unitOfWork;
    private readonly IAppCacheService _cache;

    public UpdateFecundacionInteractor(
        IGanaderiaUnitOfWork unitOfWork,
        IAppCacheService cache
    )
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<UpdateFecundacionOutput> HandleAsync(
        UpdateFecundacionCommand command,
        CancellationToken cancellationToken = default)
    {
        var repository = _unitOfWork.Fecundaciones;

        var existing = await repository.GetForEditAsync(command.Id, cancellationToken);
        if (existing is null)
            throw new FecundacionNotFoundException();

        if (!await repository.ExistsVacunoAsync(command.VacunoReceptorId, cancellationToken))
            throw new FecundacionVacunoNotFoundException();

        if (await repository.HasActiveFecundacionAsync(command.Id, command.VacunoReceptorId, cancellationToken))
            throw new FecundacionPendingActiveException();

        var values = new FecundacionUpdateValues(
            command.TipoFecundacionCode,
            command.VacunoReceptorId,
            command.TipoDonante,
            command.VacunoDonanteId,
            command.ExternoDonanteNombre,
            command.FechaProcedimiento,
            command.ResponsableNombre,
            command.ResultadoCode,
            command.EstadoFecundacionCode,
            command.ObservacionesVeterinarias,
            command.CodigoSemen,
            command.CodigoEmbrion);

        var updated = await repository.UpdateAsync(command.Id, values, cancellationToken)
            ?? throw new FecundacionNotFoundException();

        var detail = await repository.GetForEditAsync(command.Id, cancellationToken)
            ?? throw new FecundacionNotFoundException();

        await _cache.RemoveByPrefixAsync(FecundacionCacheKeys.ListarPrefix);

        return new UpdateFecundacionOutput(
            detail.Id,
            detail.Codigo,
            detail.TipoFecundacionCode,
            detail.VacunoReceptorId,
            detail.VacunoReceptorCodigo,
            detail.VacunoReceptorNombre,
            detail.TipoDonante,
            detail.VacunoDonanteId,
            detail.VacunoDonanteCodigo,
            detail.VacunoDonanteNombre,
            detail.ExternoDonanteNombre,
            detail.FechaProcedimiento,
            detail.ResponsableNombre,
            updated.ResultadoCode,
            updated.EstadoFecundacionCode,
            detail.ObservacionesVeterinarias,
            detail.CodigoSemen,
            detail.CodigoEmbrion,
            detail.ActualizadoEn,
            updated.Warning);
    }
}
