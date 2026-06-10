namespace ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Requests;

public sealed record EditarCeloRequest(
    long Id,
    string? Observaciones,
    List<string>? CaracteristicaCodes);
