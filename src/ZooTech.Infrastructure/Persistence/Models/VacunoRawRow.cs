using System;
using System.Collections.Generic;
using System.Text;

namespace ZooTech.Infrastructure.Persistence.Models;

public sealed record VacunoRawRow(
    long Id,
    string Codigo,
    string Nombre,
    DateOnly FechaNacimiento,
    string? RazaCode,
    DateTime? DeletedAt,
    DateOnly FechaRegistro,
    string? GranjaNombre,
    string? DistritoNombre,
    string? ProvinciaNombre,
    string? DepartamentoNombre,
    int TotalCount);
