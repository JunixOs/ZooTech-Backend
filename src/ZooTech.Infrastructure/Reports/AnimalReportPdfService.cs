using global::QuestPDF.Fluent;
using global::QuestPDF.Helpers;
using global::QuestPDF.Infrastructure;
using ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;

namespace ZooTech.Infrastructure.Reports;

public sealed class AnimalReportPdfService : IAnimalReportPdfService
{
    static AnimalReportPdfService()
    {
        global::QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateAnimalListPdf(ReportAnimalListOutput output)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1.5f, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(8).FontFamily("Arial"));

                page.Header().Column(column =>
                {
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Text("Reporte listado de vacunos")
                            .Bold().FontSize(18).FontColor("#802B4D");

                        row.ConstantItem(150).AlignRight().Text($"Generado: {DateTime.Now:yyyy-MM-dd HH:mm}")
                            .FontSize(8).Italic().FontColor(Colors.Grey.Darken1);
                    });

                    column.Item().PaddingTop(5).PaddingBottom(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Text(t =>
                        {
                            t.Span("Rango de fechas: ").Bold();
                            t.Span($"{output.FechaInicio:yyyy-MM-dd} al {output.FechaFin:yyyy-MM-dd}");
                        });

                        row.RelativeItem().Text(t =>
                        {
                            t.Span("Palabra clave: ").Bold();
                            t.Span(string.IsNullOrWhiteSpace(output.Keyword) ? "Todos" : output.Keyword);
                        });
                    });

                    column.Item().PaddingBottom(15);
                });

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(1.4f);
                        columns.RelativeColumn(2.2f);
                        columns.RelativeColumn(1.6f);
                        columns.RelativeColumn(2.1f);
                        columns.RelativeColumn(1.7f);
                        columns.RelativeColumn(1.5f);
                        columns.RelativeColumn(1.3f);
                        columns.RelativeColumn(2.1f);
                        columns.RelativeColumn(1.5f);
                        columns.RelativeColumn(1.6f);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderCellStyle).Text("Código").Bold().FontColor(Colors.White);
                        header.Cell().Element(HeaderCellStyle).Text("Nombre").Bold().FontColor(Colors.White);
                        header.Cell().Element(HeaderCellStyle).Text("Nacimiento").Bold().FontColor(Colors.White);
                        header.Cell().Element(HeaderCellStyle).Text("Tipo adquisición").Bold().FontColor(Colors.White);
                        header.Cell().Element(HeaderCellStyle).Text("Raza").Bold().FontColor(Colors.White);
                        header.Cell().Element(HeaderCellStyle).Text("Color").Bold().FontColor(Colors.White);
                        header.Cell().Element(HeaderCellStyle).Text("Sexo").Bold().FontColor(Colors.White);
                        header.Cell().Element(HeaderCellStyle).Text("Granja").Bold().FontColor(Colors.White);
                        header.Cell().Element(HeaderCellStyle).Text("Estado").Bold().FontColor(Colors.White);
                        header.Cell().Element(HeaderCellStyle).Text("Registro").Bold().FontColor(Colors.White);
                    });

                    foreach (var item in output.Items)
                    {
                        table.Cell().Element(BodyCellStyle).Text(item.Codigo);
                        table.Cell().Element(BodyCellStyle).Text(item.Nombre);
                        table.Cell().Element(BodyCellStyle).Text(item.FechaNacimiento.ToString("yyyy-MM-dd"));
                        table.Cell().Element(BodyCellStyle).Text(item.TipoAdquisicion);
                        table.Cell().Element(BodyCellStyle).Text(item.Raza);
                        table.Cell().Element(BodyCellStyle).Text(item.Color);
                        table.Cell().Element(BodyCellStyle).Text(item.Sexo);
                        table.Cell().Element(BodyCellStyle).Text(item.Granja);
                        table.Cell().Element(BodyCellStyle).Text(item.Estado);
                        table.Cell().Element(BodyCellStyle).Text(item.FechaRegistro.ToString("yyyy-MM-dd"));
                    }
                });

                page.Footer().AlignRight().Text(x =>
                {
                    x.Span("Página ");
                    x.CurrentPageNumber();
                    x.Span(" de ");
                    x.TotalPages();
                });
            });
        }).GeneratePdf();
    }

    private static IContainer HeaderCellStyle(IContainer container)
    {
        return container.Background("#802B4D")
            .Padding(3)
            .Border(0.5f)
            .BorderColor(Colors.Grey.Lighten1);
    }

    private static IContainer BodyCellStyle(IContainer container)
    {
        return container.Border(0.5f)
            .BorderColor(Colors.Grey.Lighten2)
            .Padding(3);
    }
}
