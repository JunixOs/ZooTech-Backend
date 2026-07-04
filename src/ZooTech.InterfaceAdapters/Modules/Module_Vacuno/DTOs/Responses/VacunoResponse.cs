using System;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;

public sealed record VacunoResponse(
    long Id,
    string Codigo,
    string Nombre,
    DateOnly FechaNacimiento,
    string AdquisicionPor,
    decimal? PrecioCompra,
    string Raza,
    string Color,
    string Sexo,
    string? CodigoPadre,
    string? CodigoMadre,
    string? Granja,
    string? Distrito,
    string? Departamento,
    string? Provincia,
    string? CodigoDistrito,
    string? AptoPara,
    DateOnly? FechaEspecificacion,
    string? Observaciones,
    string? FotoUrl,
    string Estado,
    DateTime CreadoEn,
    DateTime ActualizadoEn);
