namespace ZooTech.Application.Modules.Module_Celo.UseCases.CreateCelo;

public sealed record CreateCeloCommand(
    long VacunoId,
    long EncargadoUsuarioId,
    DateTime FechaHora,
    string? Observaciones,
    List<string> CaracteristicaCodes);
