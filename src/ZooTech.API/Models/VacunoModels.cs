namespace ZooTech.API.Models;

public sealed record VacunoResponse(
    string Codigo,
    string Nombre,
    string FechaNacimiento,
    string AdquisicionPor,
    string Raza,
    string Color,
    string Sexo,
    string CodigoPadre,
    string CodigoMadre,
    string Granja,
    string Distrito,
    string Departamento,
    string Provincia,
    string AptoPara,
    string FechaRegistroFuncion,
    string Observaciones,
    string FotoUrl,
    string Estado);

public sealed record UpdateVacunoRequest(
    string Nombre,
    string FechaNacimiento,
    string AdquisicionPor,
    string Raza,
    string Color,
    string Sexo,
    string CodigoPadre,
    string CodigoMadre,
    string Granja,
    string Distrito,
    string Departamento,
    string Provincia,
    string AptoPara,
    string FechaRegistroFuncion,
    string Observaciones,
    string FotoUrl,
    string Estado);

public sealed record PagedResponse<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages);

public sealed record VacunoOptionsResponse(
    IReadOnlyList<string> AdquisicionOptions,
    IReadOnlyList<string> RazaOptions,
    IReadOnlyList<string> SexoOptions,
    IReadOnlyList<string> AptoParaOptions);
