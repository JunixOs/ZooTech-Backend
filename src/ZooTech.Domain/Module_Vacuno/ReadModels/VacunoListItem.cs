namespace ZooTech.Domain.Module_Vacuno.ReadModels;

public sealed record VacunoListItem(
    long Id,
    string Codigo,
    string Nombre,
    DateOnly FechaNacimiento,
    DateOnly FechaRegistro,
    string RazaCode,
    string? Procedencia,
    bool IsDeleted);
