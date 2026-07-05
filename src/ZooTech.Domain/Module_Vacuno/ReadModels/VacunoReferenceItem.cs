namespace ZooTech.Domain.Module_Vacuno.ReadModels;

public sealed record VacunoReferenceItem(
    long Id,
    string Codigo,
    string Nombre,
    string SexoCode,
    string? EstadoCode);
