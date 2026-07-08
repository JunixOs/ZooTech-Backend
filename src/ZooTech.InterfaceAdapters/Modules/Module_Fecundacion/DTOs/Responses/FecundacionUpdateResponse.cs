namespace ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.DTOs.Responses;

public sealed record FecundacionUpdateResponse(
    long Id,
    string TipoFecundacion,
    VacunoResumenResponse VacunoReceptor,
    object MachoODonante,
    DateOnly FechaProcedimiento,
    string Responsable,
    string Resultado,
    string? CodigoSemen,
    string? CodigoEmbrion,
    string? Observaciones,
    DateTime ActualizadoEn,
    string? Warning);
