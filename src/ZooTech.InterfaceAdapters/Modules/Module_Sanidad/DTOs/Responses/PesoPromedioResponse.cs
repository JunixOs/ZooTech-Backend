namespace ZooTech.InterfaceAdapters.Modules.Module_Sanidad.DTOs.Responses;

public sealed record PesoPromedioResponse(IReadOnlyList<PesoPromedioItemResponse> Items);

public sealed record PesoPromedioItemResponse(
    int Anio,
    int Mes,
    decimal PesoPromedio);
