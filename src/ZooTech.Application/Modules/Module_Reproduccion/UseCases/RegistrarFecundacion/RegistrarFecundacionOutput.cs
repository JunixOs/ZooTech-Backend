namespace ZooTech.Application.Modules.Module_Reproduccion.UseCases.RegistrarFecundacion;

public sealed record RegistrarFecundacionOutput(
    long Id,
    string Codigo,
    string TipoFecundacionCode,
    long VacunoReceptorId,
    string ResultadoCode,
    DateOnly FechaProcedimiento
);
