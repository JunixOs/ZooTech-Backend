namespace ZooTech.Domain.Module_Fecundacion.ReadModels;

public sealed record FecundacionListItem(
    long Id,
    string Codigo,
    string Tipo,
    string VacunoReceptor,
    DateOnly FechaProcedimiento,
    string Responsable,
    string Resultado,
    string? Observaciones);
