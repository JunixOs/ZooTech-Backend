namespace ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.GetFecundacionEstado;

public sealed record GetFecundacionEstadoOutput(
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
