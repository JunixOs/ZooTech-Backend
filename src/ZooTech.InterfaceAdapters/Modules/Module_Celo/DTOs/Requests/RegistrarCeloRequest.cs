namespace ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Requests;

public sealed record RegistrarCeloRequest(
    long VacunoId,
    long EncargadoUsuarioId,
    DateTime FechaHora,
    string? Observaciones,
    List<string> CaracteristicaCodes);
