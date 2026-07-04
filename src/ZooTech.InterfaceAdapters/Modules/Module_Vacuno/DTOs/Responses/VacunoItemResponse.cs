namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;

public sealed record VacunoItemResponse(
    long Id,
    string Codigo,
    string Nombre,
    DateOnly FechaNacimiento,
    DateOnly FechaRegistro,
    string RazaCode,
    string? Procedencia,
    string Estado);

public sealed record VacunoReferenceResponse(
    long Id,
    string Codigo,
    string Nombre,
    string SexoCode);
