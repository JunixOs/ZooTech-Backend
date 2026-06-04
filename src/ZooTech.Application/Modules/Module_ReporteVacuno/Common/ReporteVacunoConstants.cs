namespace ZooTech.Application.Modules.Module_ReporteVacuno.Common;

public static class ReporteVacunoConstants
{
    public static readonly IReadOnlyCollection<string> FormatosPermitidos = new[] { "json", "pdf", "excel" };
    public static readonly IReadOnlyCollection<string> EstadosPermitidos = new[] { "vivo", "muerto" };
    public static readonly IReadOnlyCollection<string> AptosPermitidos = new[] { "produccion_leche", "carne", "reproduccion" };

    public const string FormatoDefault = "json";
}
