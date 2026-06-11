using FluentValidation;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;
using ZooTech.Domain.Module_ProduccionLeche.Entities;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.CreateOrdenio;

public sealed class CreateOrdenioInteractor : ICreateOrdenioInputPort
{
    private readonly IOrdenioRepository _repository;
    private readonly IValidator<CreateOrdenioCommand> _validator;

    public CreateOrdenioInteractor(IOrdenioRepository repository, IValidator<CreateOrdenioCommand> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<CreateOrdenioOutput> HandleAsync(CreateOrdenioCommand command, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);
        await OrdenioReferenceValidator.EnsureReferencesExistAsync(
            _repository,
            command.VacunoId,
            command.EncargadoUsuarioId,
            command.EstadoOrdenioCode,
            cancellationToken);

        if (await _repository.ExistsCodigoAsync(command.Codigo, cancellationToken))
        {
            throw new ConflictException("CONFLICT_CODE_EXISTS");
        }

        if (await _repository.ExistsVacunoFechaAsync(command.VacunoId, command.FechaHora, null, cancellationToken))
        {
            throw new ConflictException("CONFLICT_VACUNO_FECHA");
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
                null,
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
