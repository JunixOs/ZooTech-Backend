namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetHistorialCeloPorVacuno;

public sealed record CeloHistorialCeloPorVacunoDto(int Numero, DateTime FechaHora, bool? Resultado);

public sealed record CeloResumenReproductivoDto(
    int Celos, int Embarazos, int Fecundaciones, int Crias, int Montas, int Inseminaciones, DateOnly? UltimoParto);

public sealed record GetHistorialCeloPorVacunoOutput(
    string? Encargado,
    IReadOnlyList<CeloHistorialCeloPorVacunoDto> Historial,
    CeloResumenReproductivoDto Resumen);
