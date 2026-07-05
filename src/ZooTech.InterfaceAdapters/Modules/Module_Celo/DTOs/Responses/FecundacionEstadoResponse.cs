namespace ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Responses;

public sealed record FecundacionEstadoResponse(
    long VacunoId,
    string CodigoVacuno,
    string NombreVacuno,
    string EstadoActual,
    bool DisponibleNuevaFecundacion,
    DateTime? UltimaActualizacion,
    string? CodigoFecundacion,
    string? TipoFecundacion,
    string? ToroDonante,
    string? Responsable,
    DateOnly? FechaProcedimiento,
    string? Resultado,
    string? Observaciones);
