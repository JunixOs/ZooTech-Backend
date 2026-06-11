using FluentValidation;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.DeleteOrdenio;

public sealed class DeleteOrdenioInteractor : IDeleteOrdenioInputPort
{
    private readonly IOrdenioRepository _repository;
    private readonly IValidator<DeleteOrdenioCommand> _validator;

    public DeleteOrdenioInteractor(IOrdenioRepository repository, IValidator<DeleteOrdenioCommand> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task HandleAsync(long id, DeleteOrdenioCommand command, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);
        var existing = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("ORDENIO_NOT_FOUND", $"No se encontró el ordeño solicitado {id}.");

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
