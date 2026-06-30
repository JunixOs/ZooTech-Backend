namespace ZooTech.Application.Modules.Module_Celo.UseCases.UpdateCelo;

public sealed record UpdateCeloCommand(
    long Id,
    string? Observaciones,
    List<string>? CaracteristicaCodes);
