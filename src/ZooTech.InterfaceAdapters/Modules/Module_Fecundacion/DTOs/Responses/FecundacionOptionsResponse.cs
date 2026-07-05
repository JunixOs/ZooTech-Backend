namespace ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.DTOs.Responses;

public sealed record FecundacionOptionResponse(string Code, string Nombre, string? Descripcion);

public sealed record FecundacionOptionsResponse(
    IReadOnlyList<FecundacionOptionResponse> Tipos,
    IReadOnlyList<FecundacionOptionResponse> Resultados,
    IReadOnlyList<FecundacionOptionResponse> Estados);
