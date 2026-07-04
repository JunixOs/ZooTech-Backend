using System;
using System.Collections.Generic;
using System.Text;

namespace ZooTech.Domain.Module_Vacuno.ReadModels.ListarVacuno;

public sealed record VacunoListItem(
    long Id,
    string Codigo,
    string Nombre,
    DateOnly FechaNacimiento,
    string RazaCode,
    string? Procedencia,
    bool IsDeleted,
    DateOnly FechaRegistro);
