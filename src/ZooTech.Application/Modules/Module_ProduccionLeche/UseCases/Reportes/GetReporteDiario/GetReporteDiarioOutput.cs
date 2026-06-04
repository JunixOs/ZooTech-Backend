using System;
using System.Collections.Generic;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Reportes.GetReporteDiario;

public sealed record GetReporteDiarioOutput(IReadOnlyList<ReporteDiarioItem> Items);

public sealed record ReporteDiarioItem(DateTime Fecha, decimal TotalLitros, int CantidadOrdenios);

public sealed record GetReporteDiarioOutputPDF(byte[] PdfBytes);
