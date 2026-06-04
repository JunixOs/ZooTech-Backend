using System;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Reportes.GetReporteDiario;

public sealed record GetReporteDiarioQuery(
    DateTime? FechaDesde,
    DateTime? FechaHasta,
    long? VacunoId
);
