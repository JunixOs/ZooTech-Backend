using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.DeleteTriaje;

public sealed class DeleteTriajeInteractor : IDeleteTriajeInputPort
{
    private readonly ITriajeRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public DeleteTriajeInteractor(ITriajeRepository repository, IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task HandleAsync(long id, CancellationToken cancellationToken = default)
    {
        var triaje = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("No se encontró el triaje solicitado.");

        triaje.SoftDelete(null, _dateTimeProvider.ServerNow);
        await _repository.UpdateAsync(triaje, cancellationToken);
    }
}
