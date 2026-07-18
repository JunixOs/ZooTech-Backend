namespace ZooTech.Application.Modules.Module_Fecundacion.Common;

internal static class FecundacionCacheKeys
{
    public const string ListarPrefix = "fecundacion:listar";

    public static string Listar(
        string? query,
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        string? resultado,
        int page,
        int limit)
    {
        return string.Join(':',
            ListarPrefix,
            Normalize(query),
            FormatDate(fechaDesde),
            FormatDate(fechaHasta),
            Normalize(resultado),
            page,
            limit);
    }

    private static string Normalize(string? value)
        => string.IsNullOrWhiteSpace(value)
            ? "_"
            : value.Trim().ToUpperInvariant().Replace(':', '_');

    private static string FormatDate(DateTime? value)
        => value.HasValue ? value.Value.Date.ToString("yyyyMMdd") : "_";
}
