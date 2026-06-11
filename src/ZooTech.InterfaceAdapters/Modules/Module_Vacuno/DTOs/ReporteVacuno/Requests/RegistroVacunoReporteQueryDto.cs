using Microsoft.AspNetCore.Mvc;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.ReporteVacuno.Requests;

public sealed class RegistroVacunoReporteQueryDto
{
    [FromQuery(Name = "formato")]
    public string? Formato { get; init; }
}

