using System;
using System.Collections.Generic;
using System.Text;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;

public sealed record OrdenioListOutput(
    long? Id,
    string Codigo,
    DateTime FechaHora,
    long VacunoId,
    string NombreVacuno,
    string VacunoCodigo,
    long EncargadoUsuarioId,
    string NombreCompleto,
    decimal Litros,
    string EstadoOrdenioCode,
    string? Observaciones,
    DateTime CreatedAt,
    DateTime UpdatedAt);