using System;

namespace ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.DTOs.Requests;

public sealed record ReporteDiarioRequest(DateTime? FechaDesde, DateTime? FechaHasta, long? VacunoId);
