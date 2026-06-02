using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Ports;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.DeleteOrdenio;

public sealed class DeleteOrdenioInteractor : IDeleteOrdenioInputPort
{
    private readonly IOrdenioRepository _repository;

    public DeleteOrdenioInteractor(IOrdenioRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(long id, DeleteOrdenioCommand command, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("No se encontró el ordeño solicitado.");

        try
        {
            existing.SoftDelete(command.MotivoEliminacion, null, DateTime.UtcNow);
        }
        catch (ArgumentException ex)
        {
            throw new ConflictException(ex.Message);
        }

        _ = await _repository.UpdateAsync(existing, cancellationToken);
    }
}
