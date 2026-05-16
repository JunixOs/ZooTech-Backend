using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Ports;
using ZooTech.Domain.Module_ProduccionLeche.Entities;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.CreateOrdenio;

public sealed class CreateOrdenioInteractor : ICreateOrdenioInputPort
{
    private readonly IOrdenioRepository _repository;

    public CreateOrdenioInteractor(IOrdenioRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateOrdenioOutput> HandleAsync(CreateOrdenioCommand command, CancellationToken cancellationToken)
    {
        await OrdenioReferenceValidator.EnsureReferencesExistAsync(
            _repository,
            command.VacunoId,
            command.EncargadoUsuarioId,
            command.EstadoOrdenioCode,
            cancellationToken);

        if (await _repository.ExistsCodigoAsync(command.Codigo, cancellationToken))
        {
            throw new ConflictException("Ya existe un ordeño con el mismo código.");
        }

        if (await _repository.ExistsVacunoFechaAsync(command.VacunoId, command.FechaHora, null, cancellationToken))
        {
            throw new ConflictException("Ya existe un ordeño para el mismo vacuno en la misma fecha y hora.");
        }

        Ordenio ordenio;
        try
        {
            ordenio = Ordenio.CreateNew(
                command.Codigo,
                command.FechaHora,
                command.VacunoId,
                command.EncargadoUsuarioId,
                command.Litros,
                command.EstadoOrdenioCode,
                command.Observaciones,
                command.ActorUsuarioId,
                DateTime.UtcNow);
        }
        catch (ArgumentException ex)
        {
            throw new ConflictException(ex.Message);
        }

        var saved = await _repository.AddAsync(ordenio, cancellationToken);
        return new CreateOrdenioOutput(OrdenioMapper.ToOutput(saved));
    }
}
