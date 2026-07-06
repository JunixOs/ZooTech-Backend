using ZooTech.Application.Common.Exceptions;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.Common;

internal static class TriajeReferenceValidator
{
    public static async Task EnsureReferencesExistAsync(
        ITriajeRepository repository,
        long vacunoId,
        long? encargadoUsuarioId,
        string tipoPesoCode,
        CancellationToken cancellationToken)
    {
        if (!await repository.ExistsVacunoAsync(vacunoId, cancellationToken))
        {
            throw new ConflictException("El vacuno indicado no existe.");
        }

        if (encargadoUsuarioId.HasValue && !await repository.ExistsUsuarioAsync(encargadoUsuarioId.Value, cancellationToken))
        {
            throw new ConflictException("El usuario encargado indicado no existe.");
        }

        if (!await repository.ExistsTipoPesoAsync(tipoPesoCode, cancellationToken))
        {
            throw new ConflictException("El tipo de peso indicado no existe.");
        }
    }
}
