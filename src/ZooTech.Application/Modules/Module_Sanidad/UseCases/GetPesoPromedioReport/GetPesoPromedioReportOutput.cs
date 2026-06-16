namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetPesoPromedioReport;

public sealed record GetPesoPromedioReportOutput(IReadOnlyList<PesoPromedioItemOutput> Items);

public sealed record PesoPromedioItemOutput(
    int Anio,
    int Mes,
    decimal PesoPromedio);
