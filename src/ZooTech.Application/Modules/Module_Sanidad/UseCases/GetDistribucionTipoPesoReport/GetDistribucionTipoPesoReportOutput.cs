namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetDistribucionTipoPesoReport;

public sealed record GetDistribucionTipoPesoReportOutput(IReadOnlyList<DistribucionItemOutput> Items);

public sealed record DistribucionItemOutput(
    string TipoPesoCode,
    int Cantidad,
    decimal PesoPromedio);
