namespace ZooTech.InterfaceAdapters.Modules.Module_ReporteVacuno.DTOs.Responses;

public sealed record ErrorResponseDto(ErrorBodyDto Error);

public sealed record ErrorBodyDto(
    string Code,
    string Message,
    IReadOnlyCollection<ErrorDetailDto> Details);

public sealed record ErrorDetailDto(string Field, string Message);
