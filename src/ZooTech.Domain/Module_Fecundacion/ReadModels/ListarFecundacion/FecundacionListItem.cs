using System;
using System.Collections.Generic;
using System.Text;

namespace ZooTech.Domain.Module_Fecundacion.ReadModels;

public sealed record FecundacionListItem(
    long Id,
    string CodigoFecundacion,
    DateOnly FechaProcedimiento,
    string NombreVacunoReceptor,
    string? Responsable,
    string TipoFecundacion,
    string CodigoResultado,
    string NombreDonante);