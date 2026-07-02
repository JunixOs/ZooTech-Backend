namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;

public sealed record VacunoCatalogOptionResponse(string Code, string Nombre);

public sealed record GranjaCatalogOptionResponse(long Id, string Nombre);

public sealed record VacunoCatalogsResponse(
    IReadOnlyList<VacunoCatalogOptionResponse> TiposAdquisicion,
    IReadOnlyList<VacunoCatalogOptionResponse> Razas,
    IReadOnlyList<VacunoCatalogOptionResponse> Colores,
    IReadOnlyList<VacunoCatalogOptionResponse> Sexos,
    IReadOnlyList<VacunoCatalogOptionResponse> Utilizaciones,
    IReadOnlyList<GranjaCatalogOptionResponse> Granjas);
