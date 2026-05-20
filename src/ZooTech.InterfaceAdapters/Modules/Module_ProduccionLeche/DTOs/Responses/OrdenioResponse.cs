namespace ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.DTOs.Responses;

public sealed record OrdenioResponse(
    long Id,
    string Codigo,
    DateTime FechaHora,
    long VacunoId,
    string NombreVacuno,
    long EncargadoUsuarioId,
    decimal Litros,
    string EstadoOrdenioCode,
    string? Observaciones,
    DateTime CreatedAt,
    DateTime UpdatedAt);
