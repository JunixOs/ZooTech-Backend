using Microsoft.AspNetCore.Http;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Requests;

public sealed class VacunoMultipartRequest
{
    public string Payload { get; init; } = string.Empty;

    public IFormFile? Foto { get; init; }
}
