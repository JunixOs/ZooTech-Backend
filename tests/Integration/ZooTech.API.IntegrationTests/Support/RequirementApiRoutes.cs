namespace ZooTech.API.IntegrationTests.Support;

internal static class RequirementApiRoutes
{
    public const string Vacunos = "/api/v1/vacunos";
    public const string Fecundaciones = "/api/v1/fecundaciones";
    public const long MissingEntityId = 999999;

    public static string Vacuno(long id) => $"{Vacunos}/{id}";
    public static string VacunosPage(int page, int limit, string? query = null)
        => $"{Vacunos}?page={page}&limit={limit}"
           + (string.IsNullOrWhiteSpace(query) ? string.Empty : $"&q={Uri.EscapeDataString(query)}");
    public static string VacunoCatalogos => $"{Vacunos}/catalogos";
    public static string VacunoGenealogia(long id, int? niveles = null)
        => $"{Vacuno(id)}/genealogia"
           + (niveles.HasValue ? $"?niveles={niveles.Value}" : string.Empty);
    public static string VacunoGenealogiaExportar(long id, string? formato = null)
        => $"{VacunoGenealogia(id)}/exportar"
           + (string.IsNullOrWhiteSpace(formato) ? string.Empty : $"?formato={Uri.EscapeDataString(formato)}");
    public static string VacunoActivityStats(string fechaInicio, string fechaFin)
        => $"{Vacunos}/estadisticas/actividad?fechaInicio={Uri.EscapeDataString(fechaInicio)}&fechaFin={Uri.EscapeDataString(fechaFin)}";

    public static string Fecundacion(long id) => $"{Fecundaciones}/{id}";
    public static string FecundacionOpciones => $"{Fecundaciones}/opciones";
    public static string FecundacionVacunos(
        string sexo,
        string query,
        bool soloDisponibles = false)
        => $"{Fecundaciones}/vacunos?sexo={Uri.EscapeDataString(sexo)}"
           + $"&q={Uri.EscapeDataString(query)}"
           + $"&soloDisponibles={soloDisponibles.ToString().ToLowerInvariant()}";
}
