namespace ZooTech.Domain.Module_Sanidad.Entities;

public sealed record ResumenSanidad(
    int TotalTriajes,
    int TotalVacunosConTriajes,
    decimal PesoPromedioGeneral,
    IReadOnlyList<TipoPesoCountItem> DistribucionTipoPeso);
