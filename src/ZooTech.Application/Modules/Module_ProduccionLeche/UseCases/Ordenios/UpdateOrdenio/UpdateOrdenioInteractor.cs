using FluentValidation;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;


namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.UpdateOrdenio;

public sealed class UpdateOrdenioInteractor : IUpdateOrdenioInputPort
{
    private readonly IOrdenioRepository _repository;
    private readonly IValidator<UpdateOrdenioCommand> _validator;

    public UpdateOrdenioInteractor(IOrdenioRepository repository, IValidator<UpdateOrdenioCommand> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<UpdateOrdenioOutput> HandleAsync(long id, UpdateOrdenioCommand command, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);
        var existing = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("ORDENIO_NOT_FOUND", $"No se encontró el ordeño solicitado {id}.");

        var fechaHora = command.FechaHora ?? existing.FechaHora;
        var encargadoUsuarioId = command.EncargadoUsuarioId ?? existing.EncargadoUsuarioId;
        var litros = command.Litros ?? existing.Litros;
        var estadoOrdenioCode = string.IsNullOrWhiteSpace(command.EstadoOrdenioCode)
            ? existing.EstadoOrdenioCode
            : command.EstadoOrdenioCode;
        var observaciones = command.Observaciones ?? existing.Observaciones;

        await OrdenioReferenceValidator.EnsureReferencesExistAsync(
            _repository,
            existing.VacunoId,
            encargadoUsuarioId,
            estadoOrdenioCode,
            cancellationToken);

        if (await _repository.ExistsVacunoFechaAsync(existing.VacunoId, fechaHora, existing.Id, cancellationToken))
        {
            throw new ConflictException("ORDENIO_CONFLICT");
        }

        try
        {
            existing.Update(
                fechaHora,
                encargadoUsuarioId,
                litros,
                estadoOrdenioCode,
                observaciones,
                null,
                DateTime.UtcNow);
        }
        catch (ArgumentException ex)
        {
            throw new ConflictException(ex.Message);
        }

        var updated = await _repository.UpdateAsync(existing, cancellationToken);
        return new UpdateOrdenioOutput(OrdenioMapper.ToOutput(updated));
    }
}
