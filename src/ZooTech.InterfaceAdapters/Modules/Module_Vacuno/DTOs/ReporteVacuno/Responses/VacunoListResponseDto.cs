namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.ReporteVacuno.Responses;

public sealed record VacunoListResponseDto(
    IReadOnlyCollection<VacunoListadoItemDto> Data,
    PaginationDto Pagination,
    ReporteVacunoFiltrosDto Filtros);

public sealed record PaginationDto(
    int Page,
    int Limit,
    int Total,
    int TotalPages);

