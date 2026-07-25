namespace ZooTech.Domain.Module_Celo.Entities;

public sealed record CeloHistorialResumenItem(int Numero, DateTime FechaHora, bool? Resultado);

public sealed record CeloResumenReproductivo(
    int Celos,
    int Embarazos,
    int Fecundaciones,
    int Crias,
    int Montas,
    int Inseminaciones,
    DateOnly? UltimoParto)
{
    public static readonly CeloResumenReproductivo Empty = new(0, 0, 0, 0, 0, 0, null);
}

public sealed record CeloDetallePorVacuno(
    string? Encargado,
    IReadOnlyList<string> CaracteristicaCodes,
    IReadOnlyList<CeloHistorialResumenItem> Historial,
    CeloResumenReproductivo Resumen);
