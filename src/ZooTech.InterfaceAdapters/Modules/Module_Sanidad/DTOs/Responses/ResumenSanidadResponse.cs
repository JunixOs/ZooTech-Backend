namespace ZooTech.InterfaceAdapters.Modules.Module_Sanidad.DTOs.Responses;

public sealed record ResumenSanidadResponse(
    int TotalTriajes,
    int TotalVacunosConTriajes,
    decimal PesoPromedioGeneral,
    IReadOnlyList<TipoPesoDistribucionResponse> DistribucionTipoPeso);

public sealed record TipoPesoDistribucionResponse(
    string TipoPesoCode,
    int Cantidad,
    decimal PesoPromedio);
