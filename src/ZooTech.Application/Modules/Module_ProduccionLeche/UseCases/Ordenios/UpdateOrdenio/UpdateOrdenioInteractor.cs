using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;
using ZooTech.Domain.Shared.Enums;
using ZooTech.Domain.Shared.Interfaces;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.UpdateOrdenio;

public sealed class UpdateOrdenioInteractor : IUpdateOrdenioInputPort
{
    private readonly IGanaderiaUnitOfWork _unitOfWork;

    public UpdateOrdenioInteractor(IGanaderiaUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateOrdenioOutput> Handle(UpdateOrdenioCommand command, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Ordenios;

        var existing = await repository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new NotFoundException(
                ScopeName.Application,
                ModuleName.Produccion_Leche,
                "No se encontró el ordeño solicitado."
            );

        await OrdenioReferenceValidator.EnsureReferencesExistAsync(
            repository,
            existing.VacunoId,
            command.EncargadoUsuarioId,
            command.EstadoOrdenioCode,
            cancellationToken);

        if (await repository.ExistsVacunoFechaAsync(existing.VacunoId, command.FechaHora, existing.Id, cancellationToken))
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

        var updated = await _unitOfWork.ExecuteInTransactionAsync(
            ct => repository.UpdateAsync(existing, ct),
            cancellationToken);
        return new UpdateOrdenioOutput(OrdenioMapper.ToOutput(updated));
    }
}
