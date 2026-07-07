using System;
using System.Collections.Generic;
using System.Text;

namespace ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.DTOs.Responses;

public sealed record FecundacionItemResponse(
    long Id,
    string CodigoFecundacion,
    DateOnly FechaProcedimiento,
    string NombreVacunoReceptor,
    string? Responsable,
    string TipoFecundacion,
    string CodigoResultado,
    string NombreDonante);
