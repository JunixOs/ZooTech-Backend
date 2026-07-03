namespace ZooTech.InterfaceAdapters.Modules.Module_Sanidad.DTOs.Requests;

public sealed record UpdateTriajeRequest(
    string TipoPesoCode,
    decimal PesoKg,
    string? Observaciones,
    long? EncargadoUsuarioId);