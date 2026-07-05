namespace ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.UpdateFecundacionEstado;

public sealed record UpdateFecundacionEstadoOutput(
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
