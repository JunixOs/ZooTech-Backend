using Microsoft.AspNetCore.Mvc;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Requests;

public sealed class RegistroVacunoReporteRequest
{
    [FromQuery(Name = "formato")]
    public string? Formato { get; init; }
}

