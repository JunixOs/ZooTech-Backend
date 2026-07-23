using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Common.Models;
using ZooTech.Domain.Common.Interfaces;
using ZooTech.Domain.Shared.Enums;
using ZooTech.Domain.Shared.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.DeleteTriaje;

public sealed class DeleteTriajeInteractor : IDeleteTriajeInputPort
{
    private readonly IGanaderiaUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IEstadoRegistroRepository _estadoRegistroRepository;

    public DeleteTriajeInteractor(
        IGanaderiaUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider,
        IEstadoRegistroRepository estadoRegistroRepository
    )
    {
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
        _estadoRegistroRepository = estadoRegistroRepository;
    }

    public async Task<EmptyOutput> Handle(DeleteTriajeCommand command, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Triajes;

        var triaje = await repository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new NotFoundException(
                ScopeName.Application,
                ModuleName.Triaje,
                "No se encontró el triaje solicitado."
            );

        var estadoEliminado = await _estadoRegistroRepository.GetDeletedCodeAsync(cancellationToken);
        triaje.SoftDelete(command.MotivoEliminacion, estadoEliminado, null, _dateTimeProvider.ServerNow);
        _ = await _unitOfWork.ExecuteInTransactionAsync(
            ct => repository.UpdateAsync(triaje, ct),
            cancellationToken);

        return EmptyOutput.Value;
    }
}
