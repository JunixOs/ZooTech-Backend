using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.UpdateOrdenio;

public sealed class UpdateOrdenioInteractor : IUpdateOrdenioInputPort
{
    private readonly IOrdenioRepository _repository;

    public UpdateOrdenioInteractor(IOrdenioRepository repository)
    {
        _repository = repository;
    }

    public async Task<UpdateOrdenioOutput> Handle(UpdateOrdenioCommand command, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new NotFoundException(
                ScopeName.Application,
                ModuleName.Produccion_Leche,
                "No se encontró el ordeño solicitado."
            );

        await OrdenioReferenceValidator.EnsureReferencesExistAsync(
            _repository,
            existing.VacunoId,
            command.EncargadoUsuarioId,
            command.EstadoOrdenioCode,
            cancellationToken);

        if (await _repository.ExistsVacunoFechaAsync(existing.VacunoId, command.FechaHora, existing.Id, cancellationToken))
        {
            throw new ConflictException(
                ScopeName.Application,
                ModuleName.Produccion_Leche,
                new List<string>
                {
                    "ORDENIO-ORDENIO-VACUNO_FECHA-EXISTS"
                }
            );
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
            throw new ConflictException(
                ScopeName.Application,
                ModuleName.Produccion_Leche,
                new List<string>
                {
                    ex.Message
                }
            );
        }

        var updated = await _repository.UpdateAsync(existing, cancellationToken);
        return new UpdateOrdenioOutput(OrdenioMapper.ToOutput(updated));
    }
}
