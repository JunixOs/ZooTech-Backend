using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;
using ZooTech.Domain.Module_ProduccionLeche.Entities;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.CreateOrdenio;

public sealed class CreateOrdenioInteractor : ICreateOrdenioInputPort
{
    private readonly IOrdenioRepository _repository;

    public CreateOrdenioInteractor(IOrdenioRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateOrdenioOutput> Handle(
        CreateOrdenioCommand command, 
        CancellationToken cancellationToken
    )
    {
        await OrdenioReferenceValidator.EnsureReferencesExistAsync(
            _repository,
            command.VacunoId,
            command.EncargadoUsuarioId,
            command.EstadoOrdenioCode,
            cancellationToken);

        if (await _repository.ExistsCodigoAsync(command.Codigo, cancellationToken))
        {
            throw new ConflictException(
                ScopeName.Application,
                ModuleName.Produccion_Leche,
                new List<string>
                {
                    "ORDENIO-ORDENIO-CODE-ALREADY_EXISTS"
                }
            );
        }

        if (await _repository.ExistsVacunoFechaAsync(command.VacunoId, command.FechaHora, null, cancellationToken))
        {
            throw new ConflictException(
                ScopeName.Application,
                ModuleName.Produccion_Leche,
                new List<string>
                {
                    "ORDENIO-ORDENIO-DATE-ALREADY_EXISTS"
                }
            );
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
            throw new ConflictException(
                ScopeName.Application,
                ModuleName.Produccion_Leche,
                message: ex.Message
            );
        }

        var saved = await _repository.AddAsync(ordenio, cancellationToken);
        return new CreateOrdenioOutput(OrdenioMapper.ToOutput(saved));
    }
}
