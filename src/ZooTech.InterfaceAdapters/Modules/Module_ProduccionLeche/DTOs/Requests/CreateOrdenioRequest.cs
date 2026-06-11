namespace ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.DTOs.Requests;

public sealed record CreateOrdenioRequest(
    string Codigo,
    DateTime FechaHora,
    long VacunoId,
    long EncargadoUsuarioId,
    decimal Litros,
    string EstadoOrdenioCode,
    string? Observaciones);
