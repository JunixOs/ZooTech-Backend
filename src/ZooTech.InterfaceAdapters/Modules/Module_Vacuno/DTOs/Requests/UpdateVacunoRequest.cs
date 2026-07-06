namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Requests;

public sealed record UpdateVacunoRequest(
    string Nombre,
    DateOnly FechaNacimiento,
    string TipoAdquisicionCode,
    string RazaCode,
    string ColorCode,
    string SexoCode,
    long? PadreId,
    long? MadreId,
    long GranjaId,
    string? Observaciones);
