namespace ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.Common;

public sealed class FecundacionEstadoTransitionValidator
{
    private static readonly IReadOnlyDictionary<string, HashSet<string>> AllowedTransitions =
        new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase)
        {
            [FecundacionEstadoConstants.Pendiente] =
            [
                FecundacionEstadoConstants.EnProceso,
                FecundacionEstadoConstants.Fallida
            ],
            [FecundacionEstadoConstants.EnProceso] =
            [
                FecundacionEstadoConstants.Confirmada,
                FecundacionEstadoConstants.Fallida
            ],
            [FecundacionEstadoConstants.Confirmada] =
            [
                FecundacionEstadoConstants.EnGestacion
            ],
            [FecundacionEstadoConstants.EnEspera] =
            [
                FecundacionEstadoConstants.Gestante,
                FecundacionEstadoConstants.Vacia
            ],
            [FecundacionEstadoConstants.Gestante] =
            [
                FecundacionEstadoConstants.Vacia
            ]
        };

    public void ValidateTransition(string currentEstado, string nextEstado)
    {
        var current = FecundacionEstadoConstants.ToTransitionState(currentEstado);
        var next = FecundacionEstadoConstants.ToTransitionState(nextEstado);

        if (!AllowedTransitions.TryGetValue(current, out var nextStates) ||
            !nextStates.Contains(next))
        {
            throw new FecundacionEstadoValidationException(
                new Dictionary<string, string>
                {
                    ["estadoFecundacion"] =
                        $"No se permite cambiar el estado de '{currentEstado}' a '{nextEstado}'."
                });
        }
    }
}
