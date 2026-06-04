using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Ports;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;

internal static class OrdenioReferenceValidator
{
    public static async Task EnsureReferencesExistAsync(
        IOrdenioRepository repository,
        long vacunoId,
        long encargadoUsuarioId,
        string estadoOrdenioCode,
        CancellationToken cancellationToken)
    {
        if (!await repository.ExistsVacunoAsync(vacunoId, cancellationToken))
        {
            throw new ConflictException("El vacuno indicado no existe.");
        }

        if (!await repository.ExistsUsuarioAsync(encargadoUsuarioId, cancellationToken))
        {
            throw new ConflictException("El usuario encargado indicado no existe.");
        }

        if (!await repository.ExistsEstadoAsync(estadoOrdenioCode, cancellationToken))
        {
            throw new ConflictException("El estado de ordeño indicado no existe.");
        }
    }
}
