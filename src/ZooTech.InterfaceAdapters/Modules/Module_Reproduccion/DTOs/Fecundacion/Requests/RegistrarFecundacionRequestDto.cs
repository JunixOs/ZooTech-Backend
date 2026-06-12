namespace ZooTech.InterfaceAdapters.Modules.Module_Reproduccion.DTOs.Fecundacion.Requests;

public sealed record RegistrarFecundacionRequestDto(
    string TipoFecundacionCode,
    long VacunoReceptorId,
    long? VacunoDonanteId,
    string? NombreMachoExterno,
    DateOnly FechaProcedimiento,
    long ResponsableId,
    string? CodigoSemen,
    string? CodigoEmbrion,
    string? Observaciones
);
