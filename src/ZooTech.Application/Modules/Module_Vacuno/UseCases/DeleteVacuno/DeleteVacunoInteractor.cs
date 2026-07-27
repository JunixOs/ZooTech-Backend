using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Application.Modules.Module_Vacuno.Common;
using ZooTech.Application.Modules.Module_Vacuno.Exceptions;
using ZooTech.Domain.Module_Celo.Interfaces;
using ZooTech.Domain.Module_Sanidad.Interfaces;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;
using ZooTech.Domain.Shared.Enums;
using ZooTech.Domain.Shared.Interfaces;
using ZooTech.Application.Common.Models;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.DeleteVacuno;

public sealed class DeleteVacunoInteractor : IDeleteVacunoInputPort
{
    private readonly IGanaderiaUnitOfWork _unitOfWork;
    private readonly ICeloRepository _celoRepository;
    private readonly ITriajeRepository _triajeRepository;
    private readonly IOrdenioRepository _ordenioRepository;
    private readonly IAppCacheService _cache;

    public DeleteVacunoInteractor(
        IGanaderiaUnitOfWork unitOfWork,
        ICeloRepository celoRepository,
        ITriajeRepository triajeRepository,
        IOrdenioRepository ordenioRepository,
        IAppCacheService cache
    )
    {
        _unitOfWork = unitOfWork;
        _celoRepository = celoRepository;
        _triajeRepository = triajeRepository;
        _ordenioRepository = ordenioRepository;
        _cache = cache;
    }

    public async Task<EmptyOutput> HandleAsync(DeleteVacunoCommand command, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Vacunos;
        var existing = await repository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new VacunoNotFoundException($"No existe el vacuno con el ID {command.Id}.");

        // Validar dependencias (PDF pág 10: "Solo se elimina si no tiene registros dependientes activos: triajes, incidentes, ordeños, etc.")
        if (await _celoRepository.HasActiveRecordsByVacunoAsync(command.Id, cancellationToken))
        {
            throw new VacunoHasDependenciesException("El vacuno tiene registros de celo activos.");
        }

        if (await _ordenioRepository.HasActiveRecordsByVacunoAsync(command.Id, cancellationToken))
        {
            throw new VacunoHasDependenciesException("El vacuno tiene registros de ordeño activos.");
        }

        var triajes = await _triajeRepository.GetHistorialByVacunoIdAsync(command.Id, cancellationToken: cancellationToken);
        if (triajes != null && triajes.Any())
        {
            throw new VacunoHasDependenciesException("El vacuno tiene registros de triaje activos.");
        }

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

        _ = await _unitOfWork.ExecuteInTransactionAsync(
            ct => repository.UpdateAsync(existing, null, null, ct),
            cancellationToken);
        await _cache.RemoveByPrefixAsync(VacunoCacheKeys.ListarPrefix);

        return EmptyOutput.Value;
    }
}
