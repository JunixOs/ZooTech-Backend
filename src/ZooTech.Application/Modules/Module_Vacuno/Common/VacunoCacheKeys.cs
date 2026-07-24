namespace ZooTech.Application.Modules.Module_Vacuno.Common;

internal static class VacunoCacheKeys
{
    public const string ListarPrefix = "vacunos:listar";

    public static string Listar(
        string? query,
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        string? estado,
        int page,
        int limit)
    {
        return string.Join(':',
            ListarPrefix,
            Normalize(query),
            FormatDate(fechaDesde),
            FormatDate(fechaHasta),
            Normalize(estado),
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
