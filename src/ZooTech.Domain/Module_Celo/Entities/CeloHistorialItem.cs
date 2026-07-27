namespace ZooTech.Domain.Module_Celo.Entities;

public sealed record CeloHistorialItem(
    long Id,
    DateTime FechaHora,
    IReadOnlyList<string> CaracteristicaCodes,
    string? Observaciones,
    string? CreadoPor,
    DateTime CreadoEn,
    string? ActualizadoPor,
    DateTime ActualizadoEn);
