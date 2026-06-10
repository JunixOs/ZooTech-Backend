namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;

public sealed record VacunoItemResponse(
    long Id,
    string Codigo,
    string Nombre,
    string RazaCode,
    string SexoCode);
