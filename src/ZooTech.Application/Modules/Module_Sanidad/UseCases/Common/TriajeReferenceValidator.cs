using ZooTech.Application.Common.Exceptions;
using ZooTech.Domain.Module_Sanidad.Interfaces;
using ZooTech.Domain.Shared.Enums;

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
            throw new ConflictException(
                ScopeName.Application,
                ModuleName.Triaje,
                new List<string>()
                {
                    "TRIAJE-TRIAJE-VACUNO-NOT_EXISTS"
                },
                "El vacuno indicado no existe."
            );
        }

        if (encargadoUsuarioId.HasValue && !await repository.ExistsUsuarioAsync(encargadoUsuarioId.Value, cancellationToken))
        {
            throw new ConflictException(
                ScopeName.Application,
                ModuleName.Triaje,
                new List<string>()
                {
                    "TRIAJE-TRIAJE-ENCARGADO_USUARIO_ID-NOT_EXISTS"
                },
                "El usuario encargado indicado no existe."
            );
        }

        if (!await repository.ExistsTipoPesoAsync(tipoPesoCode, cancellationToken))
        {
            throw new ConflictException(
                ScopeName.Application,
                ModuleName.Triaje,
                new List<string>()
                {
                    "TRIAJE-TRIAJE-TIPO_PESO_CODE-NOT_EXISTS"
                },
                "El tipo de peso indicado no existe.");
        }
    }
}
