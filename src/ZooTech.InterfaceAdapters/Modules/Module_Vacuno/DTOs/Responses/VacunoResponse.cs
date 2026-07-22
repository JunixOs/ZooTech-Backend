using System;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;

public sealed record VacunoResponse(
    long Id,
    string Codigo,
    string Nombre,
    DateOnly FechaNacimiento,
    string TipoAdquisicionCode,
    string RazaCode,
    string ColorCode,
    string SexoCode,
    long? PadreId,
    long? MadreId,
    long GranjaId,
    string? Observaciones,
    DateOnly FechaRegistro,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string? CodigoPadre,
    string? CodigoMadre,
    string? Granja,
    string? Distrito,
    string? Departamento,
    string? Provincia,
    string? CodigoDistrito,
    string? AptoPara,
    DateTime? FechaUtilizacion);
