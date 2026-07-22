namespace ZooTech.Domain.Ganaderia.Module_Fecundacion.Entities;

public sealed record FecundacionListItem(
    long Id,
    string Codigo,
    string Tipo,
    string VacunoReceptor,
    DateOnly FechaProcedimiento,
    string Responsable,
    string Resultado,
    string? NombreDonante,
    string? Observaciones);
