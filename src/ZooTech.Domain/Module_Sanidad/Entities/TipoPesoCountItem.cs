namespace ZooTech.Domain.Module_Sanidad.Entities;

public sealed record TipoPesoCountItem(
    string TipoPesoCode,
    int Cantidad,
    decimal PesoPromedio);
