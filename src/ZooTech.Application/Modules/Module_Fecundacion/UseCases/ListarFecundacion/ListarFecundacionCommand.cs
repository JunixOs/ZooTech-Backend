using System;
using System.Collections.Generic;
using System.Text;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.ListarFecundacion;

public record ListarFecundacionCommand(
    string? Query = null,
    DateTime? FechaDesde = null,
    DateTime? FechaHasta = null,
    string? Resultado = null,
    int Page = 1,
    int Limit = 20);
