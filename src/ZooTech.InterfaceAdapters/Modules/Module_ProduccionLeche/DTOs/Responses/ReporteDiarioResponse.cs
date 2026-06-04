using System;
using System.Collections.Generic;

namespace ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.DTOs.Responses;

public sealed record ReporteDiarioItemResponse(DateTime Fecha, decimal TotalLitros, int CantidadOrdenios);

public sealed record GetReporteDiarioResponse(IReadOnlyList<ReporteDiarioItemResponse> Items);
