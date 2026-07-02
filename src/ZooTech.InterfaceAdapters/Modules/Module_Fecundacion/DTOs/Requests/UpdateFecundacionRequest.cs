using System.Text.Json;

namespace ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.DTOs.Requests;

public sealed record UpdateFecundacionRequest(
    string? TipoFecundacion,
    long? VacunoReceptorId,
    JsonElement? MachoODonante,
    bool? MachoExterno,
    DateOnly? FechaProcedimiento,
    string? Responsable,
    string? Resultado,
    string? EstadoFecundacion,
    string? CodigoSemen,
    string? CodigoEmbrion,
    string? Observaciones);
