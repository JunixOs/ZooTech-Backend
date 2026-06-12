namespace ZooTech.Application.Modules.Module_Reproduccion.UseCases.RegistrarFecundacion;

public sealed record RegistrarFecundacionCommand(
    string TipoFecundacionCode,
    long VacunoReceptorId,
    long? VacunoDonanteId,
    string? NombreMachoExterno,
    DateOnly FechaProcedimiento,
    long ResponsableId,
    string? CodigoSemen,
    string? CodigoEmbrion,
    string? Observaciones,
    long? CurrentUserId
);
