namespace ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.DTOs.Responses;

public sealed record CreateFecundacionResponse(
    long Id,
    string Codigo,
    DateTime FechaProcedimiento);
