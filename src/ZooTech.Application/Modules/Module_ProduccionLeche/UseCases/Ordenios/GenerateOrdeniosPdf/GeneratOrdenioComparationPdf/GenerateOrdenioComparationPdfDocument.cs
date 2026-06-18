using System;
using System.Collections.Generic;
using System.Text;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf.GeneratOrdenioComparationPdf;

public record GenerateOrdenioComparationPdfDocument(
    IReadOnlyList<OrdenioOutput> Items,
    long? VacunoId,
    string? estadoOrdenioCode,
    DateTime? FechaOrdenio
,
    DateTime? fechaHasta,
    DateTime serverNow);

