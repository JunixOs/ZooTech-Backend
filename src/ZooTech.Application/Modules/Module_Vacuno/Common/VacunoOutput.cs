namespace ZooTech.Application.Modules.Module_Vacuno.Common;

public sealed record VacunoOutput(
    long Id,
    string Codigo,
    string Nombre,
    DateOnly FechaNacimiento,
    string TipoAdquisicionCode,
    string RazaCode,
    string ColorCode,
    string SexoCode,
    long? PadreId,
    long? MadreId,
    long GranjaId,
    string? Observaciones,
    DateOnly FechaRegistro,
    DateTime CreatedAt,
    DateTime UpdatedAt);
