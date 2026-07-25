namespace ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Responses;

public sealed record CeloHistorialPorVacunoItemResponse(int Numero, DateTime FechaHora, bool? Resultado);

public sealed record CeloResumenReproductivoResponse(
    int Celos, int Embarazos, int Fecundaciones, int Crias, int Montas, int Inseminaciones, DateOnly? UltimoParto);

public sealed record CeloHistorialPorVacunoResponse(
    string? Encargado,
    IReadOnlyList<CeloHistorialPorVacunoItemResponse> Historial,
    CeloResumenReproductivoResponse Resumen);
