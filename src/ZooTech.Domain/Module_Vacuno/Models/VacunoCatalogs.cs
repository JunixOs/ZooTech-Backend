namespace ZooTech.Domain.Module_Vacuno.Models;

public sealed record VacunoCatalogOption(string Code, string Nombre);

public sealed record GranjaCatalogOption(long Id, string Nombre);

public sealed record VacunoCatalogs(
    IReadOnlyList<VacunoCatalogOption> TiposAdquisicion,
    IReadOnlyList<VacunoCatalogOption> Razas,
    IReadOnlyList<VacunoCatalogOption> Colores,
    IReadOnlyList<VacunoCatalogOption> Sexos,
    IReadOnlyList<VacunoCatalogOption> Estados,
    IReadOnlyList<VacunoCatalogOption> Utilizaciones,
    IReadOnlyList<GranjaCatalogOption> Granjas);
