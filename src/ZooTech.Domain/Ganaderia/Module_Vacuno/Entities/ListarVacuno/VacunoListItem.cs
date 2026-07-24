using System;
using System.Collections.Generic;
using System.Text;

namespace ZooTech.Domain.Ganaderia.Module_Vacuno.Entities.ListarVacuno;

public sealed record VacunoListItem(
    long Id,
    string Codigo,
    string Nombre,
    DateOnly FechaNacimiento,
    string RazaCode,
    string? Procedencia,
    bool IsDeleted,
    DateOnly FechaRegistro);
