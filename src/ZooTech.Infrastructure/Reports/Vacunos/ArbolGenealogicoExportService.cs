using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities.GetArbolGenealogico;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;
using ZooTech.Application.Common.Gateway.Services;

namespace ZooTech.Infrastructure.Reports.Vacunos;

public sealed class ArbolGenealogicoExportService : IArbolGenealogicoExportService
{
    private static readonly XLColor GreenPrimaryXl = XLColor.FromHtml("#166534");
    private static readonly XLColor GreenLightXl = XLColor.FromHtml("#f0fdf4");
    private static readonly XLColor GreenBorderXl = XLColor.FromHtml("#bbf7d0");
    private static readonly XLColor TextDarkXl = XLColor.FromHtml("#2d2d2d");

    private static readonly string GreenPrimaryPdf = "#166534";
    private static readonly string GreenLightPdf = "#f0fdf4";
    private static readonly string GreenBorderPdf = "#bbf7d0";
    private static readonly string TextDarkPdf = "#2d2d2d";

    public Task<byte[]> GenerateExcelAsync(
        List<VacunoGenealogiaNode> arbolGenealogico, Vacuno raiz, CancellationToken cancellationToken = default)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Árbol Genealógico");

        worksheet.Cell(1, 1).Value = "Nivel";
        worksheet.Cell(1, 2).Value = "Parentesco";
        worksheet.Cell(1, 3).Value = "Código";
        worksheet.Cell(1, 4).Value = "Nombre";
        worksheet.Cell(1, 5).Value = "Raza";
        worksheet.Cell(1, 6).Value = "Sexo";
        worksheet.Cell(1, 7).Value = "Procedencia";

        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Font.FontColor = XLColor.White;
        headerRow.Style.Fill.BackgroundColor = GreenPrimaryXl;
        headerRow.Style.Border.BottomBorder = XLBorderStyleValues.Medium;
        headerRow.Style.Border.BottomBorderColor = GreenBorderXl;

        var nodos = ConstruirListaAncestros(arbolGenealogico, raiz);

        int row = 2;
        foreach (var item in nodos)
        {
            var v = item.Node.Vacuno;
            worksheet.Cell(row, 1).Value = item.Node.Nivel;
            worksheet.Cell(row, 2).Value = GetRelacion(item.Node.Nivel, item.Cadena);
            worksheet.Cell(row, 3).Value = v.Codigo;
            worksheet.Cell(row, 4).Value = v.Nombre;
            worksheet.Cell(row, 5).Value = v.RazaCode ?? "-";
            worksheet.Cell(row, 6).Value = v.SexoCode ?? "-";
            worksheet.Cell(row, 7).Value = item.Node.Procedencia ?? "-";

            if (row % 2 == 0)
            {
                worksheet.Range(row, 1, row, 7).Style.Fill.BackgroundColor = GreenLightXl;
            }
            worksheet.Range(row, 1, row, 7).Style.Font.FontColor = TextDarkXl;

            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return Task.FromResult(stream.ToArray());
    }

    public Task<byte[]> GeneratePdfAsync(
        List<VacunoGenealogiaNode> arbolGenealogico, Vacuno raiz, CancellationToken cancellationToken = default)
    {
        var nodos = ConstruirListaAncestros(arbolGenealogico, raiz);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontColor(TextDarkPdf).FontFamily(Fonts.Arial));

                page.Header().Element(ComposeHeader);
                page.Content().Element(x => ComposeContent(x, nodos));
                page.Footer().Element(ComposeFooter);
            });
        });

        return Task.FromResult(document.GeneratePdf());
    }

    private void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text("Árbol Genealógico").FontSize(20).SemiBold().FontColor(GreenPrimaryPdf);
                column.Item().Text($"Generado el: {DateTime.Now:dd/MM/yyyy HH:mm}");
            });
        });
    }

    private void ComposeContent(IContainer container, List<(VacunoGenealogiaNode Node, List<string> Cadena)> nodos)
    {
        container.PaddingVertical(1, Unit.Centimetre).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(40);
                columns.RelativeColumn(3);
                columns.RelativeColumn(3);
                columns.RelativeColumn(4);
                columns.RelativeColumn(2);
                columns.RelativeColumn(2);
                columns.RelativeColumn(3);
            });

            table.Header(header =>
            {
                header.Cell().Element(StyleHeaderCell).Text("Nivel");
                header.Cell().Element(StyleHeaderCell).Text("Parentesco");
                header.Cell().Element(StyleHeaderCell).Text("Código");
                header.Cell().Element(StyleHeaderCell).Text("Nombre");
                header.Cell().Element(StyleHeaderCell).Text("Raza");
                header.Cell().Element(StyleHeaderCell).Text("Sexo");
                header.Cell().Element(StyleHeaderCell).Text("Procedencia");

                static IContainer StyleHeaderCell(IContainer container)
                {
                    return container.DefaultTextStyle(x => x.SemiBold().FontColor(Colors.White))
                        .PaddingVertical(5)
                        .PaddingHorizontal(2)
                        .Background(GreenPrimaryPdf);
                }
            });

            uint rowIndex = 1;
            foreach (var item in nodos)
            {
                var v = item.Node.Vacuno;
                var bgColor = rowIndex % 2 == 0 ? GreenLightPdf : Colors.White.ToString();

                table.Cell().Element(c => StyleRowCell(c, bgColor)).Text(item.Node.Nivel.ToString());
                table.Cell().Element(c => StyleRowCell(c, bgColor)).Text(GetRelacion(item.Node.Nivel, item.Cadena));
                table.Cell().Element(c => StyleRowCell(c, bgColor)).Text(v.Codigo);
                table.Cell().Element(c => StyleRowCell(c, bgColor)).Text(v.Nombre);
                table.Cell().Element(c => StyleRowCell(c, bgColor)).Text(v.RazaCode ?? "-");
                table.Cell().Element(c => StyleRowCell(c, bgColor)).Text(v.SexoCode ?? "-");
                table.Cell().Element(c => StyleRowCell(c, bgColor)).Text(item.Node.Procedencia ?? "-");

                rowIndex++;
            }

            static IContainer StyleRowCell(IContainer container, string bgColor)
            {
                return container.BorderBottom(1).BorderColor(GreenBorderPdf).Background(bgColor).PaddingVertical(4).PaddingHorizontal(2);
            }
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Text(x =>
        {
            x.Span("Página ");
            x.CurrentPageNumber();
            x.Span(" de ");
            x.TotalPages();
        });
    }

    private static List<(VacunoGenealogiaNode Node, List<string> Cadena)> ConstruirListaAncestros(
        List<VacunoGenealogiaNode> arbolGenealogico, Vacuno raiz)
    {
        var nodos = new List<(VacunoGenealogiaNode Node, List<string> Cadena)>();
        var raizNodo = arbolGenealogico.FirstOrDefault(n => n.Vacuno.Id == raiz.Id);
        var queue = new Queue<(VacunoGenealogiaNode Node, List<string> Cadena)>();
        queue.Enqueue((raizNodo ?? new VacunoGenealogiaNode(raiz, 1, null), new List<string>()));
        var visited = new HashSet<long> { raiz.Id };

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            nodos.Add(current);
            var v = current.Node.Vacuno;

            if (v.PadreId.HasValue && !visited.Contains(v.PadreId.Value))
            {
                var padre = arbolGenealogico.FirstOrDefault(x => x.Vacuno.Id == v.PadreId.Value);
                if (padre != null)
                {
                    visited.Add(padre.Vacuno.Id);
                    queue.Enqueue((padre, new List<string>(current.Cadena) { "Padre" }));
                }
            }

            if (v.MadreId.HasValue && !visited.Contains(v.MadreId.Value))
            {
                var madre = arbolGenealogico.FirstOrDefault(x => x.Vacuno.Id == v.MadreId.Value);
                if (madre != null)
                {
                    visited.Add(madre.Vacuno.Id);
                    queue.Enqueue((madre, new List<string>(current.Cadena) { "Madre" }));
                }
            }
        }
        return nodos;
    }

    private static string GetRelacion(int nivel, List<string> cadena)
    {
        if (nivel == 1) return "Raíz";
        if (nivel == 2) return cadena[^1];

        var esPaterno = cadena[0] == "Padre";
        var ladoTexto = esPaterno ? "paterno" : "materno";
        var ladoTextoF = esPaterno ? "paterna" : "materna";
        var esRamaFemenina = cadena[^1] == "Madre";

        return nivel switch
        {
            3 => esRamaFemenina ? $"Abuela {ladoTextoF}" : $"Abuelo {ladoTexto}",
            4 => esRamaFemenina ? $"Bisabuela {ladoTextoF}" : $"Bisabuelo {ladoTexto}",
            _ => $"Ancestro {ladoTexto} (nivel {nivel})"
        };
    }
}
