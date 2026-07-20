namespace ZooTech.API.IntegrationTests.Support;

internal static class RequirementApiRoutes
{
    public const string Vacunos = "/api/v1/vacunos";
    public const string Fecundaciones = "/api/v1/fecundaciones";

    public static string Vacuno(long id) => $"{Vacunos}/{id}";
    public static string Fecundacion(long id) => $"{Fecundaciones}/{id}";
}
