namespace ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.Common;

public static class FecundacionEstadoConstants
{
    public const string Pendiente = "Pendiente";
    public const string EnProceso = "En proceso";
    public const string Confirmada = "Confirmada";
    public const string EnGestacion = "En gestación";
    public const string Fallida = "Fallida";
    public const string EnEspera = "En Espera";
    public const string Gestante = "Gestante";
    public const string Vacia = "Vac\u00eda";
    public const string SinEstado = "Sin estado";

    private static readonly HashSet<string> KnownStates = new(StringComparer.OrdinalIgnoreCase)
    {
        Pendiente,
        EnProceso,
        Confirmada,
        EnGestacion,
        Fallida,
        EnEspera,
        Gestante,
        Vacia
    };

    public static bool IsKnownEstado(string estado)
        => KnownStates.Contains(ToTransitionState(estado));

    public static bool IsActive(string? estado)
    {
        var key = NormalizeKey(estado);
        return key is "PENDIENTE" or "PEND" or
            "EN_PROCESO" or "PROC" or
            "CONFIRMADA" or "CONF" or
            "EN_GESTACION" or "GESTACION" or
            "EN_ESPERA" or "GESTANTE";
    }

    public static bool IsDisponible(string? estado)
    {
        var key = NormalizeKey(estado);
        return key is "SIN_ESTADO" or "FALLIDA" or "VACIA";
    }

    public static string ToTransitionState(string? estado)
    {
        var key = NormalizeKey(estado);
        return key switch
        {
            "PEND" or "PENDIENTE" => Pendiente,
            "PROC" or "EN_PROCESO" => EnProceso,
            "CONF" or "CONFIRMADA" => Confirmada,
            "EN_GESTACION" or "GESTACION" => EnGestacion,
            "FALLIDA" => Fallida,
            "EN_ESPERA" => EnEspera,
            "GESTANTE" => Gestante,
            "VACIA" => Vacia,
            _ => estado?.Trim() ?? string.Empty
        };
    }

    public static IReadOnlyCollection<string> GetLookupValues(string estado)
    {
        return ToTransitionState(estado) switch
        {
            Pendiente => ["PEND", "PENDIENTE", Pendiente],
            EnProceso => ["PROC", "EN_PROCESO", EnProceso],
            Confirmada => ["CONF", "CONFIRMADA", Confirmada],
            EnGestacion => ["GESTACION", "EN_GESTACION", EnGestacion],
            Fallida => ["FALLIDA", Fallida],
            EnEspera => ["en_espera", "EN_ESPERA", EnEspera],
            Gestante => ["gestante", "GESTANTE", Gestante],
            Vacia => ["vacia", "VACIA", Vacia],
            _ => [estado.Trim()]
        };
    }

    private static string NormalizeKey(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return value
            .Trim()
            .Replace(" ", "_")
            .Replace("-", "_")
            .Replace("Ã­", "I")
            .Replace("í", "I")
            .Replace("Í", "I")
            .Replace("Ã³", "O")
            .Replace("ó", "O")
            .Replace("Ó", "O")
            .ToUpperInvariant();
    }
}
