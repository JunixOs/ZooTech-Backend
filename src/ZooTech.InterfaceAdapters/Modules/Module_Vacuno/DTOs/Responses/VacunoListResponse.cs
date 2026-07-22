namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;

public sealed record VacunoListResponse(
    IReadOnlyCollection<VacunoListadoItemResponse> Data,
    PaginationResponse Pagination,
    ReporteVacunoFiltrosResponse Filtros);

public sealed record PaginationResponse(
    int Page,
    int Limit,
    int Total,
    int TotalPages);

