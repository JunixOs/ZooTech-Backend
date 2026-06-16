namespace ZooTech.InterfaceAdapters.Modules.Module_Sanidad.DTOs.Responses;

public sealed record DistribucionTipoPesoResponse(IReadOnlyList<DistribucionItemResponse> Items);

public sealed record DistribucionItemResponse(
    string TipoPesoCode,
    int Cantidad,
    decimal PesoPromedio);
