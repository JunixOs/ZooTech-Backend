using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Ports;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.UpdateOrdenio;

public sealed class UpdateOrdenioInteractor : IUpdateOrdenioInputPort
{
    private readonly IOrdenioRepository _repository;

    public UpdateOrdenioInteractor(IOrdenioRepository repository)
    {
        _repository = repository;
    }

    public async Task<UpdateOrdenioOutput> HandleAsync(long id, UpdateOrdenioCommand command, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("No se encontró el ordeño solicitado.");

        await OrdenioReferenceValidator.EnsureReferencesExistAsync(
            _repository,
            existing.VacunoId,
            command.EncargadoUsuarioId,
            command.EstadoOrdenioCode,
            cancellationToken);

        if (await _repository.ExistsVacunoFechaAsync(existing.VacunoId, command.FechaHora, existing.Id, cancellationToken))
        {
            throw new ConflictException("Ya existe un ordeño para el mismo vacuno en la misma fecha y hora.");
        }

        try
        {
            existing.Update(
                command.FechaHora,
                command.EncargadoUsuarioId,
                command.Litros,
                command.EstadoOrdenioCode,
                command.Observaciones,
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
