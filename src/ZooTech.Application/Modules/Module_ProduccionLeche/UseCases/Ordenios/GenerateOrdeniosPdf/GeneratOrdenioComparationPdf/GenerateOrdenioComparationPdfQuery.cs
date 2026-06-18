namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf.GeneratOrdenioComparationPdf;

public sealed record GenerateOrdenioComparationPdfQuery(
    long? VacunoId,
    Decimal? LitrosLeche,
    DateTime? FechaDesde,
    DateTime? FechaHasta);
