namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;

public sealed record VacunoReferenceResponse(long Id, string Codigo, string Nombre, string SexoCode, string? EstadoCode);
