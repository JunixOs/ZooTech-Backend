namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;

public sealed record VacunoItemResponse(
    long Id,
    string Codigo,
    string Nombre,
    DateOnly FechaNacimiento,
    string RazaCode,
    string SexoCode,
    string? Procedencia,
    string Estado,
    DateOnly FechaRegistro);
