namespace ZooTech.InterfaceAdapters.Modules.Module_Sanidad.DTOs.Requests;

public sealed record TriajeRequest(
    long VacunoId,
    string TipoPesoCode,
    decimal PesoKg,
    string? Observaciones,
    long? EncargadoUsuarioId,
    DateTime FechaHora);