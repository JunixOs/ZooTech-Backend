using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Common.Models;
using ZooTech.Domain.Common.Interfaces;
using ZooTech.Domain.Module_Sanidad.Interfaces;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.DeleteTriaje;

public sealed class DeleteTriajeInteractor : IDeleteTriajeInputPort
{
    private readonly ITriajeRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IEstadoRegistroRepository _estadoRegistroRepository;

    public DeleteTriajeInteractor(
        ITriajeRepository repository,
        IDateTimeProvider dateTimeProvider,
        IEstadoRegistroRepository estadoRegistroRepository
    )
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
        _estadoRegistroRepository = estadoRegistroRepository;
    }

    public async Task<EmptyOutput> Handle(DeleteTriajeCommand command, CancellationToken cancellationToken)
    {
        var triaje = await _repository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new NotFoundException(
                ScopeName.Application,
                ModuleName.Triaje,
                "No se encontró el triaje solicitado."
            );

        var estadoEliminado = await _estadoRegistroRepository.GetDeletedCodeAsync(cancellationToken);
        triaje.SoftDelete(command.MotivoEliminacion, estadoEliminado, null, _dateTimeProvider.ServerNow);
        await _repository.UpdateAsync(triaje, cancellationToken);

        return EmptyOutput.Value;
    }
}
