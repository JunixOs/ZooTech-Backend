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
    string Estado,
    string? CodigoDistrito = null,
    string? CodigoDepartamento = null,
    string? CodigoProvincia = null);

public sealed record RegistrarVacunoRequest(
    string Codigo,
    string Nombre,
    DateOnly FechaNacimiento,
    string AdquisicionPor,
    decimal? PrecioCompra,
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
    DateOnly FechaEspecificacion,
    string? Observaciones,
    string? FotoUrl,
    string? CodigoDistrito = null,
    string? CodigoDepartamento = null,
    string? CodigoProvincia = null);

public sealed record RegistrarVacunoResponse(
    string Codigo,
    string Nombre,
    DateOnly FechaNacimiento,
    string AdquisicionPor,
    decimal? PrecioCompra,
    string? FotoUrl,
    DateTime CreadoEn);

public sealed record DeleteVacunoRequest(
    string? MotivoEliminacion);

public sealed record DeleteVacunoResponse(
    string Codigo,
    string Nombre,
    string MotivoEliminacion,
    DateTime FechaEliminacion);

public sealed record ApiErrorEnvelope(ApiError Error);

public sealed record ApiError(
    string Code,
    string Message,
    IReadOnlyList<ApiValidationError> Details);

public sealed record ApiValidationError(
    string Field,
    string Message);

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
    IReadOnlyList<string> AptoParaOptions,
    IReadOnlyList<UbigeoOptionResponse> UbigeoOptions);

public sealed record UbigeoOptionResponse(
    string Codigo,
    string Nombre,
    IReadOnlyList<UbigeoOptionResponse> Hijos);

public sealed record VerVacunoParametros(
    bool ExcluirEliminados = true,
    bool PermitirBusquedaCodigoSinSeparadores = true);

public sealed record VacunoActividadParametros(
    bool ContarEliminadosHastaFechaEliminacion = true);

public sealed record VacunoActividadPointResponse(
    DateOnly Fecha,
    int Cantidad);

public sealed record VacunoActividadStatsResponse(
    DateOnly FechaInicio,
    DateOnly FechaFin,
    IReadOnlyList<VacunoActividadPointResponse> Points,
    int Mayor,
    int Menor);

public sealed record VacunoGenealogiaResponse(
    long Id,
    string Codigo,
    string Nombre,
    string Raza,
    string Procedencia,
    string Sexo,
    int Nivel,
    VacunoGenealogiaResponse? Padre,
    VacunoGenealogiaResponse? Madre);
