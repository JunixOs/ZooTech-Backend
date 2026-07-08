namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionForEdit;

public sealed record GetFecundacionForEditOutput(
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
    long? ExternoDonanteId,
    string? ExternoDonanteNombre,
    DateOnly FechaProcedimiento,
    string ResponsableNombre,
    string ResultadoCode,
    string EstadoFecundacionCode,
    string? ObservacionesVeterinarias,
    string? CodigoSemen,
    string? CodigoEmbrion,
    DateTime CreadoEn,
    DateTime ActualizadoEn);
