using System;
using System.Collections.Generic;
using System.Text;

namespace ZooTech.Domain.Ganaderia.Module_Vacuno.Entities.GetArbolGenealogico;

public sealed record GetArbolGenealogicoItem(
    long Id,
    string Codigo,
    string Nombre,
    DateOnly FechaNacimiento,
    string RazaCode,
    string? Procedencia,
    long? PadreId,
    long? MadreId,
    int Nivel);
