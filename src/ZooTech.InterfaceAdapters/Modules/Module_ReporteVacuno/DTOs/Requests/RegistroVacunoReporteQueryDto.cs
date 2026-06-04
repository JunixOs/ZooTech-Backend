using Microsoft.AspNetCore.Mvc;

namespace ZooTech.InterfaceAdapters.Modules.Module_ReporteVacuno.DTOs.Requests;

public sealed class RegistroVacunoReporteQueryDto
{
    [FromQuery(Name = "formato")]
    public string? Formato { get; init; }
}
