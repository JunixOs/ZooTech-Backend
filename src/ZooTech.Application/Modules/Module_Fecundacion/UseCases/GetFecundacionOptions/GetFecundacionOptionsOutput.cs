namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionOptions;

public sealed record FecundacionOptionOutput(string Code, string Nombre, string? Descripcion);

public sealed record GetFecundacionOptionsOutput(
    IReadOnlyList<FecundacionOptionOutput> Tipos,
    IReadOnlyList<FecundacionOptionOutput> Resultados,
    IReadOnlyList<FecundacionOptionOutput> Estados);
