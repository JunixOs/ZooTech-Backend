namespace ZooTech.Domain.Module_Fecundacion.Rules;

public static class FecundacionRules
{
    public const string TipoDonanteInterno = "INTERNO";
    public const string TipoDonanteExterno = "EXTERNO";

    public static bool EsInseminacionArtificial(string tipoCode)
        => MatchesAny(tipoCode, "IA", "INSEMINACION", "INSEMINACION_ARTIFICIAL");

    public static bool EsTransferenciaEmbriones(string tipoCode)
        => MatchesAny(tipoCode, "TE", "TRANSFERENCIA", "TRANSFERENCIA_EMBRION", "TRANSFERENCIA_EMBRIONES");

    public static bool EsMontaNatural(string tipoCode)
        => MatchesAny(tipoCode, "MN", "MONTA", "MONTA_NATURAL");

    public static bool EsResultadoExitoso(string resultadoCode)
        => MatchesAny(resultadoCode, "EXITOSA", "EXITOSO", "EXITO");

    public static bool EsResultadoFallido(string resultadoCode)
        => MatchesAny(resultadoCode, "FALLIDA", "FALLIDO", "FALLA");

    public static bool EsEstadoPendienteOConfirmacion(string estadoCode)
        => MatchesAny(
            estadoCode,
            "PENDIENTE",
            "EN_PROCESO",
            "CONFIRMADA",
            "GESTACION",
            "EN_GESTACION",
            "EN_ESPERA",
            "GESTANTE");

    private static bool MatchesAny(string value, params string[] candidates)
    {
        var normalized = Normalize(value);
        return candidates.Any(candidate => normalized == Normalize(candidate));
    }

    private static string Normalize(string value)
        => value.Trim().Replace(" ", "_").Replace("-", "_").ToUpperInvariant();
}
