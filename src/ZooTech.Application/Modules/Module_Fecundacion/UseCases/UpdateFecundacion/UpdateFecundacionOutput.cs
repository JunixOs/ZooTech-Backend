namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.UpdateFecundacion;

public sealed record UpdateFecundacionOutput(
    long Id,
    string Codigo,
    string TipoFecundacionCode,
    long VacunoReceptorId,
    string VacunoReceptorCodigo,
    string VacunoReceptorNombre,
    string TipoDonante,
    long? VacunoDonanteId,
    string? VacunoDonanteCodigo,
    string? VacunoDonanteNombre,
    string? ExternoDonanteNombre,
    DateOnly FechaProcedimiento,
    string ResponsableNombre,
    string ResultadoCode,
    string EstadoFecundacionCode,
    string? ObservacionesVeterinarias,
    string? CodigoSemen,
    string? CodigoEmbrion,
    DateTime ActualizadoEn,
    string? Warning);
