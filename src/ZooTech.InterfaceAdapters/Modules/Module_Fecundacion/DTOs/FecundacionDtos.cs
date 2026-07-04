namespace ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.DTOs;

public sealed record CreateFecundacionRequest(
    string TipoFecundacionCode,
    long VacunoReceptorId,
    long? VacunoDonanteId,
    bool MachoExterno,
    string? MachoExternoNombre,
    DateOnly FechaProcedimiento,
    string ResponsableName,
    string ResultadoCode,
    string? Observaciones);

public sealed record FecundacionResponse(
    long Id,
    string Codigo,
    string Tipo,
    string VacunoReceptor,
    DateOnly FechaProcedimiento,
    string Responsable,
    string Resultado,
    string? Observaciones);

public sealed record DeleteFecundacionRequest(string? Razon);
