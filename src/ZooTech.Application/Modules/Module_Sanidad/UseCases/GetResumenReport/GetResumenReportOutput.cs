namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetResumenReport;

public sealed record GetResumenReportOutput(
    int TotalTriajes,
    int TotalVacunosConTriajes,
    decimal PesoPromedioGeneral,
    IReadOnlyList<TipoPesoReportItem> DistribucionTipoPeso);

public sealed record TipoPesoReportItem(
    string TipoPesoCode,
    int Cantidad,
    decimal PesoPromedio);
