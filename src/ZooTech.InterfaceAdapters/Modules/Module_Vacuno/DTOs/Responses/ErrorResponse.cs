namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;

public sealed record ErrorResponse(ErrorBodyResponse Error);

public sealed record ErrorBodyResponse(
    string Code,
    string Message,
    IReadOnlyCollection<ErrorDetailResponse> Details);

public sealed record ErrorDetailResponse(string Field, string Message);

