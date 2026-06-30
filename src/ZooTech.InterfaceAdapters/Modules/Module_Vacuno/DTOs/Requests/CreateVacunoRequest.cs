namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Requests;

public sealed record CreateVacunoRequest(
    string Codigo,
    string Nombre,
    DateOnly FechaNacimiento,
    string TipoAdquisicionCode,
    string RazaCode,
    string ColorCode,
    string SexoCode,
    string? CodigoPadre,
    string? CodigoMadre,
    long? GranjaId,
    string? Granja,
    string? Distrito,
    string? Departamento,
    string? Provincia,
    string? CodigoDistrito,
    string? Observaciones);
