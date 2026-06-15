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
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                // Header
                page.Header().Column(column =>
                {
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Text("Reporte listado de vacunos")
                            .Bold().FontSize(18).FontColor("#802B4D"); // Matches primary theme color

                        row.ConstantItem(150).AlignRight().Text($"Generado: {DateTime.Now:yyyy-MM-dd HH:mm}")
                            .FontSize(8).Italic().FontColor(Colors.Grey.Darken1);
                    });

                    column.Item().PaddingTop(5).PaddingBottom(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Text(t =>
                        {
                            t.Span("Rango de Fechas: ").Bold();
                            t.Span($"{output.FechaInicio:yyyy-MM-dd} al {output.FechaFin:yyyy-MM-dd}");
                        });

                        row.RelativeItem().Text(t =>
                        {
                            t.Span("Palabra Clave: ").Bold();
                            t.Span(string.IsNullOrWhiteSpace(output.Keyword) ? "Todos" : output.Keyword);
                        });
                    });

                    column.Item().PaddingBottom(15);
                });

                // Content (Table)
                page.Content().Table(table =>
                {
                    // Define columns
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2);    // Código
                        columns.RelativeColumn(3);    // Nombre
                        columns.RelativeColumn(2);    // Raza
                        columns.RelativeColumn(1.5f); // Sexo
                        columns.RelativeColumn(3);    // Procedencia
                        columns.RelativeColumn(2);    // Estado
                        columns.RelativeColumn(2);    // Fecha registro
                    });

                    // Table Header
                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Código").Bold().FontColor(Colors.White);
                        header.Cell().Element(CellStyle).Text("Nombre").Bold().FontColor(Colors.White);
                        header.Cell().Element(CellStyle).Text("Raza").Bold().FontColor(Colors.White);
                        header.Cell().Element(CellStyle).Text("Sexo").Bold().FontColor(Colors.White);
                        header.Cell().Element(CellStyle).Text("Procedencia").Bold().FontColor(Colors.White);
                        header.Cell().Element(CellStyle).Text("Estado").Bold().FontColor(Colors.White);
                        header.Cell().Element(CellStyle).Text("Fecha registro").Bold().FontColor(Colors.White);

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.Background("#802B4D")
                                            .Padding(5)
                                            .Border(0.5f)
                                            .BorderColor(Colors.Grey.Lighten1);
                        }
                    });

                    // Table Body Rows
                    foreach (var item in output.Items)
                    {
                        table.Cell().Element(CellStyle).Text(item.Codigo);
                        table.Cell().Element(CellStyle).Text(item.Nombre);
                        table.Cell().Element(CellStyle).Text(item.Raza);
                        table.Cell().Element(CellStyle).Text(item.Sexo);
                        table.Cell().Element(CellStyle).Text(item.Procedencia);
                        table.Cell().Element(CellStyle).Text(item.Estado);
                        table.Cell().Element(CellStyle).Text(item.FechaRegistro.ToString("yyyy-MM-dd"));

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.Border(0.5f)
                                            .BorderColor(Colors.Grey.Lighten2)
                                            .Padding(5);
                        }
                    }
                });

                // Footer
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
}
