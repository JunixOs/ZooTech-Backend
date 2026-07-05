namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.CreateFecundacion;

public sealed record CreateFecundacionCommand(
    string TipoFecundacionCode,
    long VacunoReceptorId,
    long? CeloRegistroId,
    DateTime FechaProcedimiento,
    string ResponsableName,
    string ResultadoCode,
    string? ObservacionesVeterinarias,
    bool MachoExterno,
    string? MachoExternoNombre,
    long? VacunoDonanteId,
    long? CreatedById,
    string? CodigoSemen = null,
    string? CodigoEmbrion = null);
