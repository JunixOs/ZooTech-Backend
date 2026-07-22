using ZooTech.Application.Common.Exceptions;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;
using ZooTech.Domain.Shared.Enums;

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
        List<string> errors = new List<string>();

        if (!await repository.ExistsVacunoAsync(vacunoId, cancellationToken))
        {
            errors.Add("ORDENIO-ORDENIO-VACUNO-NOT_EXISTS");
        }

        if (!await repository.ExistsUsuarioAsync(encargadoUsuarioId, cancellationToken))
        {
            errors.Add("ORDENIO-ORDENIO-USUARIO-NOT_EXISTS");
        }

        if (!await repository.ExistsEstadoAsync(estadoOrdenioCode, cancellationToken))
        {
            errors.Add("ORDENIO-ORDENIO-ESTADO_ORDENIO_CODE-NOT_EXISTS");
        }

        if(errors.Count != 0)
        {
            throw new ConflictException(
                ScopeName.Application,
                ModuleName.Produccion_Leche,
                errors
            );
        }
    }
}
