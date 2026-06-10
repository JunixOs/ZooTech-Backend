namespace ZooTech.Application.Modules.Module_Celo.UseCases.RegistrarCelo;

public sealed record RegistrarCeloCommand(
    long VacunoId,
    long EncargadoUsuarioId,
    DateTime FechaHora,
    string? Observaciones,
    List<string> CaracteristicaCodes);
