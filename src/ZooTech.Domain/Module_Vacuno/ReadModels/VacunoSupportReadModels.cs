namespace ZooTech.Domain.Module_Vacuno.ReadModels;

public sealed record VacunoCodeLookup(long Id, string Codigo);

public sealed record VacunoGranjaListItem(
    long Id,
    string Nombre,
    string CodigoDistrito,
    string? Distrito,
    string? Provincia,
    string? Departamento);

public sealed record VacunoGranjaDetails(
    string Nombre,
    string CodigoDistrito,
    string? Distrito,
    string? Provincia,
    string? Departamento);

public sealed record VacunoUtilizacionDetails(
    string TipoUtilizacionCode,
    DateTime CreatedAt);
