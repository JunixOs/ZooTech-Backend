namespace ZooTech.Application.Modules.Module_Celo.UseCases.EditarCelo;

public sealed record EditarCeloCommand(
    long Id,
    string? Observaciones,
    List<string>? CaracteristicaCodes);
